using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Core.Motion;
using LiteFramework.Core.ObjectPool;

namespace LiteFramework.Core
{
    public static class LiteCoreManager
    {
        public static bool Startup(UnityEngine.MonoBehaviour Instance)
        {
            LLogger.LInfo($"{nameof(EventManager)} Startup");
            if (!EventManager.Startup())
            {
                LLogger.LError($"{nameof(EventManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(ObjectPoolManager)} Startup");
            if (!ObjectPoolManager.Startup())
            {
                LLogger.LError($"{nameof(ObjectPoolManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(TaskManager)} Startup");
            if (!TaskManager.Startup(Instance))
            {
                LLogger.LError($"{nameof(TaskManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(TimerManager)} Startup");
            if (!TimerManager.Startup())
            {
                LLogger.LError($"{nameof(TimerManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(MotionManager)} Startup");
            if (!MotionManager.Startup())
            {
                LLogger.LError($"{nameof(MotionManager)} Startup Failed");
                return false;
            }

            return true;
        }

        public static void Shutdown()
        {
            MotionManager.Shutdown();
            TimerManager.Shutdown();
            TaskManager.Shutdown();
            ObjectPoolManager.Shutdown();
            EventManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            EventManager.Tick(DeltaTime);
            ObjectPoolManager.Tick(DeltaTime);
            TaskManager.Tick(DeltaTime);
            TimerManager.Tick(DeltaTime);
            MotionManager.Tick(DeltaTime);
        }
    }
}