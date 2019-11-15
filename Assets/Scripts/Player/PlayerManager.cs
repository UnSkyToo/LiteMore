using LiteFramework;
using LiteFramework.Core.Event;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Wave;
using LiteMore.Data;
using LiteMore.UI;
using LiteMore.UI.Logic;
using UnityEngine;

namespace LiteMore.Player
{
    public static class PlayerManager
    {
        public static PlayerInfo Player => Player_;
        public static PlayerDps Dps => Dps_;
        public static CoreNpc Master => CoreNpc_;

        private static PlayerInfo Player_;
        private static PlayerDps Dps_;
        private static CoreNpc CoreNpc_;
        private static BulletCircleEmitter MainEmitter_;

        public static bool Startup()
        {
            Player_ = new PlayerInfo();
            Player_.LoadFromCache();

            Dps_ = new PlayerDps(0.5f);

            CoreNpc_ = NpcManager.AddCoreNpc("Core", Configure.CoreBasePosition, NpcManager.GenerateCoreNpcAttr());

            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
            EventManager.Register<NpcDieEvent>(OnNpcDieEvent);
            EventManager.Register<NpcAttrChangedEvent>(OnCoreAttrChanged);

            UIManager.OpenUI<MainUI>();
            //UIManager.OpenUI<DpsUI>();
            UIManager.OpenUI<HeroListUI>();
            //UIManager.OpenUI<QuickControlUI>();

            //CreateMainEmitter();
            //WaveManager.LoadWave((uint)Player.Wave);

            EventManager.Send<CoreInfoChangeEvent>();
            return true;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
            EventManager.UnRegister<NpcDieEvent>(OnNpcDieEvent);
            EventManager.UnRegister<NpcAttrChangedEvent>(OnCoreAttrChanged);
            Player_.SaveToCache();

            //UIManager.CloseUI<QuickControlUI>();
            UIManager.CloseUI<HeroListUI>();
            //UIManager.CloseUI<DpsUI>();
            UIManager.CloseUI<MainUI>();
        }

        public static void Tick(float DeltaTime)
        {
            if (CoreNpc_.CalcFinalAttr(NpcAttrIndex.Hp) <= 0)
            {
                UIManager.OpenUI<GameOverUI>();
                LiteManager.IsPause = true;
            }

            Dps_.Tick(DeltaTime);
        }

        public static void SaveToArchive()
        {
            Player_.SaveToCache();
        }

        public static bool DeleteArchive()
        {
            return Player_.DeleteArchive();
        }

        private static void OnNpcDamageEvent(NpcDamageEvent Event)
        {
            if (Event.Master.Team == CombatTeam.B)
            {
                Dps_.ApplyDamage(Event.SourceName, Event.Damage);
            }
        }

        private static void OnNpcDieEvent(NpcDieEvent Event)
        {
            if (Event.Master.Team == CombatTeam.B)
            {
                AddGem((int)Event.Master.CalcFinalAttr(NpcAttrIndex.Gem));
                SfxManager.PlayNpcSfx(Event.Master, true, "prefabs/sfx/goldsfx.prefab");
            }
        }

        public static void CreateMainEmitter()
        {
            EmitterManager.RemoveEmitter(MainEmitter_);
            MainEmitter_ = EmitterManager.AddEmitter(new BulletCircleEmitter("MainBullet")
            {
                Master = PlayerManager.Master,
                Team = CombatTeam.A,
                TriggerCount = -1,
                EmittedCountAttr = new EmitterRandInt((int)GetBulletCount()),
                Interval = GetBulletInterval(),
                IsAlive = true,
                IsPause = false,
                Position = Configure.CoreTopPosition,
                RadiusAttr = new EmitterRandFloat(0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandFloat(GetBulletDamage()),
                ResName = "Red",
            }) as BulletCircleEmitter;

            EventHelper.RemoveAllEvent(GameObject.Find("Touch").transform, true);
            EventHelper.AddEvent(GameObject.Find("Touch").transform, () =>
            {
                var Target = FilterHelper.FindNearest(Master);
                if (Target != null)
                {
                    var Desc = new TrackBulletDescriptor(
                        new BaseBulletDescriptor("Touch", Configure.CoreTopPosition, CombatTeam.A, 5),
                        "Blue", Target, 1500);

                    BulletManager.AddTrackBullet(Desc);
                }
            });
        }

        private static void OnCoreAttrChanged(NpcAttrChangedEvent Event)
        {
            if (Event.Master.ID != Master.ID)
            {
                return;
            }

            switch (Event.Index)
            {
                case NpcAttrIndex.Hp:
                    Player_.Hp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.MaxHp:
                    Player_.MaxHp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.AddHp:
                    Player_.AddHp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.Mp:
                    Player_.Mp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.MaxMp:
                    Player_.MaxMp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.AddMp:
                    Player_.AddMp = CoreNpc_.CalcFinalAttr(Event.Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                default:
                    break;
            }
        }

        public static void AddGem(int Value)
        {
            Player_.Gem += Value;
            EventManager.Send<CoreInfoChangeEvent>();
        }

        public static void AddWave()
        {
            Player_.Wave++;
            WaveManager.LoadWave((uint)Player.Wave);
            Player_.SaveToCache();
            LocalCache.SaveCache();
        }

        public static void AddBulletDamageLevel()
        {
            Player_.BulletDamageLevel++;
            MainEmitter_.DamageAttr = new EmitterRandFloat(LocalData.MainBullet.Damage[Player_.BulletDamageLevel].Damage);
            Player_.SaveToCache();
        }

        public static uint GetBulletDamageLevel()
        {
            return Player_.BulletDamageLevel;
        }

        public static float GetBulletDamage()
        {
            return LocalData.MainBullet.Damage[Player_.BulletDamageLevel].Damage;
        }

        public static uint GetBulletDamageCost()
        {
            return LocalData.MainBullet.Damage[Player_.BulletDamageLevel].Cost;
        }

        public static void AddBulletIntervalLevel()
        {
            Player_.BulletIntervalLevel++;
            MainEmitter_.Interval = LocalData.MainBullet.Interval[Player_.BulletIntervalLevel].Interval;
            Player_.SaveToCache();
        }

        public static uint GetBulletIntervalLevel()
        {
            return Player_.BulletIntervalLevel;
        }

        public static float GetBulletInterval()
        {
            return LocalData.MainBullet.Interval[Player_.BulletIntervalLevel].Interval;
        }

        public static uint GetBulletIntervalCost()
        {
            return LocalData.MainBullet.Interval[Player_.BulletIntervalLevel].Cost;
        }

        public static void AddBulletCountLevel()
        {
            Player_.BulletCountLevel++;
            MainEmitter_.EmittedCountAttr = new EmitterRandInt((int)LocalData.MainBullet.Count[Player_.BulletCountLevel].Count);
            Player_.SaveToCache();
        }

        public static uint GetBulletCountLevel()
        {
            return Player_.BulletCountLevel;
        }

        public static uint GetBulletCount()
        {
            return LocalData.MainBullet.Count[Player_.BulletCountLevel].Count;
        }

        public static uint GetBulletCountCost()
        {
            return LocalData.MainBullet.Count[Player_.BulletCountLevel].Cost;
        }
    }
}