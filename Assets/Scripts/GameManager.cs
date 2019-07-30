using System.Collections.Generic;
using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Skill.Selector;
using LiteMore.Data;
using LiteMore.Helper;
using LiteMore.Motion;
using LiteMore.Player;
using LiteMore.UI;
using LiteMore.UI.Logic;
using UnityEngine;

namespace LiteMore
{
    public static class GameManager
    {
        public static bool IsPause { get; set; } = false;
        public static bool IsRestart { get; set; } = false;
        public static float TimeScale { get; set; } = 1.0f;

        private static float EnterBackgroundTime_ = 0.0f;

        public static bool Startup()
        {
            IsPause = true;
            IsRestart = false;
            TimeScale = 1.0f;

            LocalCache.LoadCache();
            LocalData.Generate();
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

            //CreateMainSkill();

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

            LocalCache.SaveCache();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public static void Tick(float DeltaTime)
        {
            if (IsRestart)
            {
                RestartGameManager();
                return;
            }

            if (IsPause)
            {
                return;
            }

            var Dt = DeltaTime * TimeScale;
            TimerManager.Tick(Dt);
            MotionManager.Tick(Dt);
            UIManager.Tick(Dt);
            CombatManager.Tick(Dt);
            PlayerManager.Tick(Dt);
        }

        public static void Restart()
        {
            IsRestart = true;
        }

        private static void RestartGameManager()
        {
            IsRestart = false;
            UnityHelper.ClearLog();
            Shutdown();
            IsPause = !Startup();
        }

        public static void OnEnterBackground()
        {
            LocalCache.SaveCache();
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

        private static void CreateMainSkill()
        {
            var Skill1 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill1", "镭射激光", 5, 40));
            var Skill1Selector = new ClickSelector(Skill1);
            Skill1Selector.OnUsed += (S) =>
            {
                var BulletDesc = new LaserBulletDescriptor(
                    new BaseBulletDescriptor("Laser", Configure.CoreTopPosition, CombatTeam.A, 100),
                    180, 360, 300, 500);
                BulletManager.AddLaserBullet(BulletDesc);
            };

            var Skill2 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill2", "自动弹幕", 8, 30));
            var Skill2Selector = new ClickSelector(Skill2);
            Skill2Selector.OnUsed += (S) =>
            {
                EmitterManager.AddEmitter(new BulletCircleEmitter("AutoS")
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
                    DamageAttr = new EmitterRandFloat(3, 5),
                    ResName = "Blue",
                });
            };

            var Skill3 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill3", "放马过来", 3, 10));
            var Skill3Selector = new ClickSelector(Skill3);
            Skill3Selector.OnUsed += (S) =>
            {
                EmitterManager.AddEmitter(new NpcCircleEmitter("Comeon")
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
                    HpAttr = new EmitterRandFloat(1, 5),
                    DamageAttr = new EmitterRandFloat(1, 1),
                    GemAttr = new EmitterRandInt(1, 1),
                });
            };

            var Skill4 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill4", "天降正义", 5, 30, 250));
            var Skill4Selector = new DragPositionSelector(new DragSelectorDescriptor(Skill4, "Prefabs/bv0"));
            Skill4Selector.OnUsed += (Desc, Pos) =>
            {
                var BulletDesc = new BombBulletDescriptor(
                    new BaseBulletDescriptor("Bomb", new Vector2(Pos.x, 400), CombatTeam.A, 500),
                    Pos, 200, Desc.Skill.Radius);

                BulletManager.AddBombBullet(BulletDesc);
            };

            var Skill5 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill5", "速速退散", 1, 1, 128));
            var Skill5Selector = new DragDirectionSelector(new DragSelectorDescriptor(Skill5, "Prefabs/bv1"), new Vector2(Configure.CoreTopPosition.x - 50, 0));
            Skill5Selector.OnUsed += (Desc, Dir) =>
            {
                var BulletDesc = new BackBulletDescriptor(
                    new BaseBulletDescriptor("Back", new Vector2(Configure.CoreTopPosition.x - 50, 0), CombatTeam.A, 1),
                    Dir, Configure.WindowWidth,
                    new Vector2(400, 82),
                    200);

                BulletManager.AddBackBullet(BulletDesc);
            };

            var Skill6 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill6", "减速陷阱", 1, 1, 100));
            var Skill6Selector = new DragPositionSelector(new DragSelectorDescriptor(Skill6, "Prefabs/bv0"));
            Skill6Selector.OnUsed += (Desc, Pos) =>
            {
                var BulletDesc = new AttrTriggerBulletDescriptor(
                    new BaseTriggerBulletDescriptor(
                        new BaseBulletDescriptor("Slow", Pos, CombatTeam.A, 1),
                        Desc.Skill.Radius,
                        0.5f,
                        20,
                        new Color(90.0f / 255.0f, 220.0f / 255.0f, 1.0f)),
                    new List<NpcAttrModifyInfo>
                    {
                        new NpcAttrModifyInfo(NpcAttrIndex.Speed, 0.5f, 0),
                    });

                BulletManager.AddAttrTriggerBullet(BulletDesc);
            };

            var Skill7 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill7", "召唤援军", 10, 50));
            var Skill7Selector = new ClickSelector(Skill7);
            Skill7Selector.OnUsed += (S) =>
            {
                var Npc = NpcManager.AddNpc("Partner", Configure.CoreBasePosition, CombatTeam.A,
                    NpcManager.GenerateInitAttr(200, 500, 0, 1, 0, 50, 50),
                    true);
                Npc.Scale = new Vector2(3, 3);
            };

            var Skill8 = SkillManager.AddMainSkill(new BaseSkillDescriptor("Textures/Icon/skill8", "持续子弹", 0, 2));
            var Skill8Selector = new PressedSelector(Skill8, 0.1f);
            Skill8Selector.OnUsed += (S) =>
            {
                var Target = NpcManager.GetRandomNpc(CombatTeam.B);
                if (Target != null)
                {
                    BulletManager.AddTrackBullet(new TrackBulletDescriptor(
                        new BaseBulletDescriptor("Continue", Configure.CoreTopPosition, CombatTeam.A, 1),
                        "Blue",
                        Target,
                        1500));
                }
            };
        }
    }
}