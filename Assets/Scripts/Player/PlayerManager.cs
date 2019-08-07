using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Wave;
using LiteMore.Data;
using LiteMore.Extend;
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
            CoreNpc_.Attr.AttrChanged += OnCoreAttrChanged;

            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);

            UIManager.OpenUI<MainUI>();
            UIManager.OpenUI<DpsUI>();
            //UIManager.OpenUI<QuickControlUI>();

            EventManager.Send<CoreInfoChangeEvent>();
            return true;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
            Player_.SaveToCache();

            //UIManager.CloseUI<QuickControlUI>();
            UIManager.CloseUI<DpsUI>();
            UIManager.CloseUI<MainUI>();
        }

        public static void Tick(float DeltaTime)
        {
            if (CoreNpc_.CalcFinalAttr(NpcAttrIndex.Hp) <= 0)
            {
                UIManager.OpenUI<GameOverUI>();
                GameManager.IsPause = true;
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

        public static void CreateMainEmitter()
        {
            EmitterManager.RemoveEmitter(MainEmitter_);
            MainEmitter_ = EmitterManager.AddEmitter(new BulletCircleEmitter("MainBullet")
            {
                Team = CombatTeam.A,
                TriggerCount = -1,
                EmittedCount = GetBulletCount(),
                Interval = GetBulletInterval(),
                IsAlive = true,
                IsPause = false,
                Position = Configure.CoreTopPosition,
                RadiusAttr = new EmitterRandFloat(0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandFloat(GetBulletDamage()),
                ResName = "Red",
            }) as BulletCircleEmitter;

            UIEventTriggerListener.Remove(GameObject.Find("Touch").transform);
            UIEventTriggerListener.Get(GameObject.Find("Touch").transform).AddCallback(UIEventType.Click, () =>
            {
                var Target = NpcManager.GetRandomNpc(CombatTeam.B);
                if (Target != null)
                {
                    var Desc = new TrackBulletDescriptor(
                        new BaseBulletDescriptor("Touch", Configure.CoreTopPosition, CombatTeam.A, 5),
                        "Blue", Target, 1500);

                    BulletManager.AddTrackBullet(Desc);
                }
            });
        }

        private static void OnCoreAttrChanged(NpcAttrIndex Index)
        {
            switch (Index)
            {
                case NpcAttrIndex.Hp:
                    Player_.Hp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.MaxHp:
                    Player_.MaxHp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.AddHp:
                    Player_.AddHp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.Mp:
                    Player_.Mp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.MaxMp:
                    Player_.MaxMp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<CoreInfoChangeEvent>();
                    break;
                case NpcAttrIndex.AddMp:
                    Player_.AddMp = CoreNpc_.CalcFinalAttr(Index);
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

        public static void AddHp(float Value)
        {
            CoreNpc_.Attr.AddValue(NpcAttrIndex.Hp, Value);
        }

        public static void AddMp(float Value)
        {
            CoreNpc_.Attr.AddValue(NpcAttrIndex.Mp, Value);
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
            MainEmitter_.EmittedCount = LocalData.MainBullet.Count[Player_.BulletCountLevel].Count;
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