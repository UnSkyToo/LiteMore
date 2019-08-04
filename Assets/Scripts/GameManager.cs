using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.Skill;
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

        private static void TestSkill()
        {
            /*AddMainSkill(SkillLibrary.Get(1001));
            AddMainSkill(SkillLibrary.Get(1002));
            AddMainSkill(SkillLibrary.Get(1003));
            AddMainSkill(SkillLibrary.Get(1004));
            AddMainSkill(SkillLibrary.Get(1005));
            AddMainSkill(SkillLibrary.Get(1006));
            AddMainSkill(SkillLibrary.Get(1007));
            AddMainSkill(SkillLibrary.Get(1008));*/

            SkillManager.AddPassiveSkill(SkillLibrary.Get(0001), -1);
        }
    }
}