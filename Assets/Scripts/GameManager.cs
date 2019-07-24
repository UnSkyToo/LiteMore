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
            var flag = false;

            if (flag)
            {
                EmitterManager.AddEmitter(new NpcNormalEmitter
                {
                    TriggerCount = 1,
                    EmittedCount = 1,
                    Interval = 1.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = new Vector2(Configure.WindowLeft + 200, 0),
                    RadiusAttr = new EmitterRandFloat(40, 100),
                    SpeedAttr = new EmitterRandFloat(500, 500),
                    HpAttr = new EmitterRandInt(50, 50),
                    DamageAttr = new EmitterRandInt(1, 1),
                });
            }
            else
            {
                EmitterManager.AddEmitter(new NpcNormalEmitter
                {
                    Team = CombatTeam.B,
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
                    Team = CombatTeam.B,
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
                    Team = CombatTeam.B,
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
                    Team = CombatTeam.A,
                    TriggerCount = -1,
                    EmittedCount = 5,
                    Interval = 5.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = Configure.CoreTopPosition,
                    RadiusAttr = new EmitterRandFloat(0, 0),
                    SpeedAttr = new EmitterRandFloat(1000, 2000),
                    DamageAttr = new EmitterRandInt(1, 3),
                    ResName = "Red",
                });
            }
        }

        private static void CreateSkill()
        {
            var Skill1 = SkillManager.AddClickSkill("Textures/skill1", new SkillDescriptor("镭射激光", 5, 40));
            Skill1.OnUsed += (Desc) =>
            {
                var BulletDesc = new LaserBulletDescriptor(
                    new BaseBulletDescriptor(Configure.CoreTopPosition, CombatTeam.A, 100),
                    180, 360, 300, 500);
                BulletManager.AddLaserBullet(BulletDesc);
            };

            var Skill2 = SkillManager.AddClickSkill("Textures/skill2", new SkillDescriptor("自动弹幕", 8, 30));
            Skill2.OnUsed += (Desc) =>
            {
                EmitterManager.AddEmitter(new BulletNormalEmitter
                {
                    Team = CombatTeam.A,
                    TriggerCount = 10,
                    EmittedCount = 100,
                    Interval = 30.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = Configure.CoreTopPosition,
                    RadiusAttr = new EmitterRandFloat(0, 0),
                    SpeedAttr = new EmitterRandFloat(1000, 2000),
                    DamageAttr = new EmitterRandInt(3, 5),
                    ResName = "Blue",
                });
            };

            var Skill3 = SkillManager.AddClickSkill("Textures/skill3", new SkillDescriptor("放马过来", 3, 10));
            Skill3.OnUsed += (Desc) =>
            {
                EmitterManager.AddEmitter(new NpcNormalEmitter
                {
                    Team = CombatTeam.B,
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

            var Skill4 = SkillManager.AddDragPositionSkill("Textures/skill4", new SkillDescriptor("天降正义", 5, 30, 250));
            Skill4.OnUsed += (Desc, Pos) =>
            {
                var BulletDesc = new BombBulletDescriptor(
                    new BaseBulletDescriptor(new Vector2(Pos.x, 400), CombatTeam.A, 500),
                    Pos, 200, Desc.Radius);

                BulletManager.AddBombBullet(BulletDesc);
            };

            var Skill5 = SkillManager.AddDragDirectionSkill("Textures/skill5", new SkillDescriptor("速速退散", 1, 1), new Vector2(Configure.CoreTopPosition.x - 50, 0));
            Skill5.OnUsed += (Desc, Dir) =>
            {
                var BulletDesc = new BackBulletDescriptor(
                    new BaseBulletDescriptor(new Vector2(Configure.CoreTopPosition.x - 50, 0), CombatTeam.A, 1),
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
                        new BaseBulletDescriptor(Pos, CombatTeam.A, 1),
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

            var Skill7 = SkillManager.AddClickSkill("Textures/skill7", new SkillDescriptor("召唤援军", 10, 50));
            Skill7.OnUsed += (Desc) =>
            {
                var Npc = NpcManager.AddNpc(Configure.CoreBasePosition, CombatTeam.A,
                    NpcManager.GenerateInitAttr(200, 500, 0, 1, 0, 50, 50),
                    true);
                Npc.Scale = new Vector2(3, 3);
            };
        }
    }
}