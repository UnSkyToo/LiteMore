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

        private static PlayerInfo Player_;
        private static CoreNpc CoreNpc_;
        private static BulletCircleEmitter MainEmitter_;

        public static bool Startup()
        {
            Player_ = new PlayerInfo();
            Player_.LoadFromCache();

            UIManager.OpenUI<MainUI>();

            CoreNpc_ = NpcManager.AddCoreNpc(Configure.CoreBasePosition, NpcManager.GenerateCoreNpcAttr());
            CoreNpc_.Attr.AttrChanged += OnCoreAttrChanged;

            EventManager.Send<CoreInfoChangeEvent>();
            CreateMainEmitter();

            WaveManager.LoadWave((uint)Player.Wave);

            return true;
        }

        public static void Shutdown()
        {
            Player_.SaveToCache();
            UIManager.CloseUI<MainUI>();
        }

        public static void Tick(float DeltaTime)
        {
            if (CoreNpc_.CalcFinalAttr(NpcAttrIndex.Hp) <= 0)
            {
                GameManager.GameOver();
            }
        }

        public static void CreateMainEmitter()
        {
            EmitterManager.RemoveEmitter(MainEmitter_);
            MainEmitter_ = EmitterManager.AddEmitter(new BulletCircleEmitter
            {
                Team = CombatTeam.A,
                TriggerCount = -1,
                EmittedCount = 1,
                Interval = GetBulletInterval(),
                IsAlive = true,
                IsPause = false,
                Position = Configure.CoreTopPosition,
                RadiusAttr = new EmitterRandFloat(0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(GetBulletDamage()),
                ResName = "Red",
            }) as BulletCircleEmitter;

            UIEventTriggerListener.Remove(GameObject.Find("Touch").transform);
            UIEventTriggerListener.Get(GameObject.Find("Touch").transform).AddCallback(UIEventType.Click, () =>
            {
                var Target = NpcManager.GetRandomNpc(CombatTeam.B);
                if (Target != null)
                {
                    var Desc = new TrackBulletDescriptor(
                        new BaseBulletDescriptor(Configure.CoreTopPosition, CombatTeam.A, 5),
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
        }

        public static void AddBulletDamageLevel()
        {
            Player_.BulletDamageLevel++;
            MainEmitter_.DamageAttr = new EmitterRandInt(LocalData.MainBulletDamage[Player_.BulletDamageLevel].Damage);
        }

        public static int GetBulletDamage()
        {
            return LocalData.MainBulletDamage[Player_.BulletDamageLevel].Damage;
        }

        public static int GetBulletDamageCost()
        {
            return LocalData.MainBulletDamage[Player_.BulletDamageLevel].Cost;
        }

        public static void AddBulletIntervalLevel()
        {
            Player_.BulletIntervalLevel++;
            MainEmitter_.Interval = LocalData.MainBulletInterval[Player_.BulletIntervalLevel].Interval;
        }

        public static float GetBulletInterval()
        {
            return LocalData.MainBulletInterval[Player_.BulletIntervalLevel].Interval;
        }

        public static int GetBulletIntervalCost()
        {
            return LocalData.MainBulletInterval[Player_.BulletIntervalLevel].Cost;
        }

        public static void AddBulletPerCountLevel()
        {
            Player_.BulletPerCountLevel++;
            //ainEmitter_.EmittedCount = Player_.BulletPerCount;
        }
    }
}