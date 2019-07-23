using System.Collections.Generic;
using LiteMore.Combat;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
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
            /*EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = 1,
                EmittedCount = 1,
                Interval = 1.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, 0),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(50, 50),
                HpAttr = new EmitterRandInt(50, 50),
                DamageAttr = new EmitterRandInt(1, 1),
            });*/

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
                DamageAttr = new EmitterRandInt(1, 1),
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
                DamageAttr = new EmitterRandInt(1, 1),
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
                DamageAttr = new EmitterRandInt(1, 1),
            });

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
            var Skill1 = SkillManager.AddClickSkill("Textures/skill1", new SkillDescriptor("镭射激光", 5, 40));
            Skill1.OnUsed += (Desc) =>
            {
                var BulletDesc = new LaserBulletDescriptor(
                    new BaseBulletDescriptor(MapManager.BuildPosition, 100),
                    180, 360, 300, 500);
                BulletManager.AddLaserBullet(BulletDesc);
            };
            //Skill1.Tips = "<color=#ffffff><size=30>围绕核心旋转激光180°,伤害100</size></color>";

            var Skill2 = SkillManager.AddClickSkill("Textures/skill2", new SkillDescriptor("自动弹幕", 8, 30));
            Skill2.OnUsed += (Desc) =>
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

            var Skill3 = SkillManager.AddClickSkill("Textures/skill3", new SkillDescriptor("放马过来", 3, 10));
            Skill3.OnUsed += (Desc) =>
            {
                EmitterManager.AddEmitter(new NpcNormalEmitter
                {
                    TriggerCount = 1,
                    EmittedCount = 100,
                    Interval = 1.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = new Vector2(Configure.WindowLeft + 200, 0),
                    RadiusAttr = new EmitterRandFloat(10, 200),
                    SpeedAttr = new EmitterRandFloat(80, 180),
                    HpAttr = new EmitterRandInt(1, 5),
                    DamageAttr = new EmitterRandInt(1, 1),
                });
            };
            //Skill3.Tips = "<color=#ffffff><size=30>召唤100个敌方</size></color>";

            var Skill4 = SkillManager.AddDragPositionSkill("Textures/skill4", new SkillDescriptor("天降正义", 5, 30, 250));
            Skill4.OnUsed += (Desc, Pos) =>
            {
                var BulletDesc = new BombBulletDescriptor(
                    new BaseBulletDescriptor(new Vector2(Pos.x, 400), 500),
                    Pos, 200, Desc.Radius);

                BulletManager.AddBombBullet(BulletDesc);
            };
            //Skill4.Tips = "<color=#ffffff><size=30>召唤一颗从天而降的核弹\n速度较慢但伤害很高(500)\n伤害半径250</size></color>";

            var Skill5 = SkillManager.AddDragDirectionSkill("Textures/skill5", new SkillDescriptor("速速退散", 1, 1), new Vector2(MapManager.BuildPosition.x - 50, 0));
            Skill5.OnUsed += (Desc, Dir) =>
            {
                var BulletDesc = new BackBulletDescriptor(
                    new BaseBulletDescriptor(new Vector2(MapManager.BuildPosition.x - 50, 0), 1),
                    Dir, Configure.WindowWidth,
                    new Vector2(400, 82),
                    200);

                BulletManager.AddBackBullet(BulletDesc);
            };

            var Skill6 = SkillManager.AddDragPositionSkill("Textures/skill6", new SkillDescriptor("减速陷阱", 1, 1, 100));
            Skill6.OnUsed += (Desc, Pos) =>
            {
                var BulletDesc = new AttrTriggerBulletDescriptor(
                    new BaseTriggerBulletDescriptor(
                        new BaseBulletDescriptor(Pos, 1),
                        Desc.Radius,
                        0.5f,
                        20,
                        new Color(90.0f / 255.0f, 220.0f / 255.0f, 1.0f)),
                    new List<NpcAttrModifyInfo>
                    {
                        new NpcAttrModifyInfo(NpcAttrIndex.Speed, 0.5f, 0),
                    });

                BulletManager.AddAttrTriggerBullet(BulletDesc);
            };
        }
    }
}