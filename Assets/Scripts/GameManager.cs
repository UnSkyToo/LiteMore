using LiteMore.Combat;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Helper;
using LiteMore.Motion;
using LiteMore.UI;
using LiteMore.UI.Logic;
using UnityEngine;

namespace LiteMore
{
    public static class GameManager
    {
        public static bool IsPause { get; set; } = false;

        private static float EnterBackgroundTime_ = 0.0f;

        public static bool Startup()
        {
            IsPause = true;

            Lang.Load();

            if (!EventManager.Startup()
                || !TimerManager.Startup()
                || !MotionManager.Startup()
                || !UIManager.Startup()
                || !CombatManager.Startup()
                || !PlayerManager.Startup())
            {
                return false;
            }

            CreateEmitter();
            CreateSkill();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            IsPause = false;
            return true;
        }

        public static void Shutdown()
        {
            PlayerManager.Shutdown();
            CombatManager.Shutdown();
            UIManager.Shutdown();
            MotionManager.Shutdown();
            TimerManager.Shutdown();
            EventManager.Shutdown();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public static void Tick(float DeltaTime)
        {
            if (IsPause)
            {
                return;
            }

            TimerManager.Tick(DeltaTime);
            MotionManager.Tick(DeltaTime);
            UIManager.Tick(DeltaTime);
            CombatManager.Tick(DeltaTime);
            PlayerManager.Tick(DeltaTime);
        }

        public static void Restart()
        {
            UnityHelper.ClearLog();
            Shutdown();
            IsPause = !Startup();
        }

        public static void OnEnterBackground()
        {
            EnterBackgroundTime_ = Time.realtimeSinceStartup;
            IsPause = true;
        }

        public static void OnEnterForeground()
        {
            if (Time.realtimeSinceStartup - EnterBackgroundTime_ >= Configure.EnterBackgroundMaxTime)
            {
                Restart();
                return;
            }

            EnterBackgroundTime_ = Time.realtimeSinceStartup;
            IsPause = false;
        }

        public static void GameOver()
        {
            IsPause = true;
            UIManager.OpenUI<GameOverUI>();
        }

        private static void CreateEmitter()
        {
            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 10,
                Interval = 120.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, 0),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(50, 100),
                HpAttr = new EmitterRandInt(20, 40),
            });

            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 3,
                Interval = 15.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, -Configure.WindowHeight / 3.0f),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(100, 200),
                HpAttr = new EmitterRandInt(1, 5),
            });

            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 3,
                Interval = 15.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, Configure.WindowHeight / 3.0f),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(100, 200),
                HpAttr = new EmitterRandInt(1, 5),
            });

            /*EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Screen.width / 2 - 100, Screen.height / 4.0f),
                RadiusAttr = new EmitterRandFloat(5, 30),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
            });

            EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Screen.width / 2 - 100, -Screen.height / 4.0f),
                RadiusAttr = new EmitterRandFloat(5, 30),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
            });*/

            EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = MapManager.BuildPosition,
                RadiusAttr = new EmitterRandFloat(0, 0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
                ResName = "Red",
            });
        }

        private static void CreateSkill()
        {
            var Skill1 = SkillManager.AddSkill("Textures/skill1", new SkillDescriptor("镭射激光", 5, 40));
            Skill1.OnClick += () =>
            {
                BulletManager.AddLaserBullet(MapManager.BuildPosition);
            };
            //Skill1.Tips = "<color=#ffffff><size=30>围绕核心旋转激光180°,伤害100</size></color>";

            var Skill2 = SkillManager.AddSkill("Textures/skill2", new SkillDescriptor("自动弹幕", 8, 30));
            Skill2.OnClick += () =>
            {
                EmitterManager.AddEmitter(new BulletNormalEmitter
                {
                    TriggerCount = 10,
                    EmittedCount = 100,
                    Interval = 30.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = MapManager.BuildPosition,
                    RadiusAttr = new EmitterRandFloat(0, 0),
                    SpeedAttr = new EmitterRandFloat(1000, 2000),
                    DamageAttr = new EmitterRandInt(3, 5),
                    ResName = "Blue",
                });
            };
            //Skill2.Tips = "<color=#ffffff><size=30>布置一个自动发射的弹幕\n持续5秒,每次发射100个伤害\n为3-5点的子弹</size></color>";

            var Skill3 = SkillManager.AddSkill("Textures/skill3", new SkillDescriptor("放马过来", 3, 10));
            Skill3.OnClick += () =>
            {
                for (var Index = 0; Index < 100; ++Index)
                {
                    var npc = NpcManager.AddNpc(new Vector2(Random.Range(Configure.WindowLeft, -100), Random.Range(Configure.WindowBottom, Configure.WindowTop)));
                    npc.Speed = Random.Range(80, 180);
                    npc.MoveTo(new Vector2(Configure.WindowRight - 100, Random.Range(-100, 100)));
                }
            };
            //Skill3.Tips = "<color=#ffffff><size=30>召唤100个敌方</size></color>";

            var Skill4 = SkillManager.AddSkill("Textures/skill4", new SkillDescriptor("天降正义", 5, 30));
            Skill4.OnClick += () =>
            {
                var Target = NpcManager.GetRandomNpc();
                if (Target != null)
                {
                    BulletManager.AddBombBullet(Target.Position);
                }
            };
            //Skill4.Tips = "<color=#ffffff><size=30>召唤一颗从天而降的核弹\n速度较慢但伤害很高(500)\n伤害半径250</size></color>";
        }
    }
}