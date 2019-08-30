using LiteFramework.Core;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Extend.Debug;
using LiteFramework.Game;
using LiteFramework.Helper;
using LiteFramework.Interface;
using UnityEngine;

namespace LiteFramework
{
    public static class LiteManager
    {
        public static bool IsPause { get; set; } = false;
        public static bool IsRestart { get; private set; } = false;
        public static float TimeScale { get; set; } = 1.0f;

        private static MonoBehaviour MonoBehaviourInstance { get; set; }
        private static float EnterBackgroundTime_ = 0.0f;

        public static bool Startup(MonoBehaviour Instance, ILogic Logic)
        {
            IsPause = true;
            IsRestart = false;
            TimeScale = 1.0f;
            MonoBehaviourInstance = Instance;

            LiteConfigure.UIDescList.Clear();

            if (Debug.isDebugBuild ||
                Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.LinuxEditor)
            {
                Attach<Debugger>(MonoBehaviourInstance.gameObject);
            }

            LLogger.LInfo("Lite Framework Startup");
            if (!LiteCoreManager.Startup(MonoBehaviourInstance))
            {
                return false;
            }

            if (!LiteGameManager.Startup(Logic))
            {
                return false;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            IsPause = false;
            return true;
        }

        public static void Shutdown()
        {
            LiteGameManager.Shutdown();
            LiteCoreManager.Shutdown();

            if (Debug.isDebugBuild ||
                Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.LinuxEditor)
            {
                Detach<Debugger>(MonoBehaviourInstance.gameObject);
            }

            PlayerPrefs.Save();
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
            LiteCoreManager.Tick(Dt);
            LiteGameManager.Tick(Dt);
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
            IsPause = !Startup(MonoBehaviourInstance, LiteGameManager.MainLogic);
        }

        public static T Attach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                return Component;
            }

            return Root.AddComponent<T>();
        }

        public static void Detach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                Object.DestroyImmediate(Component);
            }
        }

        public static void OnEnterForeground()
        {
            EventManager.Send<EnterForegroundEvent>();

            if (Time.realtimeSinceStartup - EnterBackgroundTime_ >= LiteConfigure.EnterBackgroundMaxTime)
            {
                Restart();
                return;
            }

            EnterBackgroundTime_ = Time.realtimeSinceStartup;
            IsPause = false;
        }

        public static void OnEnterBackground()
        {
            EventManager.Send<EnterBackgroundEvent>();
            EnterBackgroundTime_ = Time.realtimeSinceStartup;
            IsPause = true;
        }
    }
}