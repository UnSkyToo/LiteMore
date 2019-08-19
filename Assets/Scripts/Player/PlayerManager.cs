using LiteFramework;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Game.UI;
using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Skill.Selector;
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
            CoreNpc_.Attr.AttrChanged += OnCoreAttrChanged;

            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
            EventManager.Register<NpcDieEvent>(OnNpcDieEvent);

            UIManager.OpenUI<MainUI>();
            UIManager.OpenUI<DpsUI>();
            //UIManager.OpenUI<QuickControlUI>();
            var UI = UIManager.OpenUI<JoystickUI>();
            var S = NpcManager.AddNpc("s", new Vector2(0, 0), CombatTeam.A, NpcManager.GenerateInitAttr(200, 1000, 0, 50, 1, 50, 50));
            S.Scale = new Vector2(3, 3);
            ((AINpc) S).EnableAI(false);

            S.AddNpcSkill(3002);
            S.AddNpcSkill(2006).Cost = 0;
            S.AddNpcSkill(2005).Cost = 0;

            UI.OnJoystickMoveEvent += (IsStop, Dir, Strength) =>
            {
                if (IsStop)
                {
                    S.StopMove();
                }
                else
                {
                    S.MoveTo(Dir * 1000, true);
                }
            };

            UI.BindSkill(0, S.GetSkill(3001));
            UI.BindSkill(1, S.GetSkill(3002));
            UI.BindSkill(2, S.GetSkill(2006));
            UI.BindSkill(3, S.GetSkill(2005));

            //CreateMainEmitter();
            //WaveManager.LoadWave((uint)Player.Wave);

            EventManager.Send<CoreInfoChangeEvent>();
            return true;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
            EventManager.UnRegister<NpcDieEvent>(OnNpcDieEvent);
            Player_.SaveToCache();

            UIManager.CloseUI<JoystickUI>();
            //UIManager.CloseUI<QuickControlUI>();
            UIManager.CloseUI<DpsUI>();
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
            if (Event.MasterTeam == CombatTeam.B)
            {
                Dps_.ApplyDamage(Event.SourceName, Event.Damage);
            }
        }

        private static void OnNpcDieEvent(NpcDieEvent Event)
        {
            if (Event.MasterTeam == CombatTeam.B)
            {
                var Npc = NpcManager.FindNpc(Event.MasterTeam, Event.MasterID);
                if (Npc != null)
                {
                    AddGem((int)Npc.CalcFinalAttr(NpcAttrIndex.Gem));
                    SfxManager.AddSfx("prefabs/sfx/goldsfx.prefab", Npc.Position);
                }
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

            UIEventTriggerListener.Remove(GameObject.Find("Touch").transform);
            UIEventTriggerListener.Get(GameObject.Find("Touch").transform).AddCallback(UIEventType.Click, () =>
            {
                var Target = LockingHelper.FindNearest(Master);
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