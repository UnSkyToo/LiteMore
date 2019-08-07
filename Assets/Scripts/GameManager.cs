using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Wave;
using LiteMore.Data;
using LiteMore.Helper;
using LiteMore.Motion;
using LiteMore.Player;
using LiteMore.UI;
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

            TestSkill();

            //PlayerManager.CreateMainEmitter();
            //WaveManager.LoadWave((uint)PlayerManager.Player.Wave);

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

            PlayerManager.SaveToArchive();
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

        private static void TestSkill()
        {
            SkillManager.AddMainSkill(SkillLibrary.Get(2001));
            /*SkillManager.AddMainSkill(SkillLibrary.Get(2002));
            SkillManager.AddMainSkill(SkillLibrary.Get(2003));
            SkillManager.AddMainSkill(SkillLibrary.Get(2004));
            SkillManager.AddMainSkill(SkillLibrary.Get(2005));
            SkillManager.AddMainSkill(SkillLibrary.Get(2006));
            SkillManager.AddMainSkill(SkillLibrary.Get(2007));
            SkillManager.AddMainSkill(SkillLibrary.Get(2008));*/

            NpcManager.AddNpc("boss", new Vector2(-Screen.width / 2, 0), CombatTeam.B,
                NpcManager.GenerateInitAttr(10, 1000, 0, 50, 1, 5, 5));

            SkillManager.AddPassiveSkill(SkillLibrary.Get(1005), -1);
        }
    }
}