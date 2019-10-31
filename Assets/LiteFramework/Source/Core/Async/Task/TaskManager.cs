using System;
using System.Collections;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Task
{
    public static class TaskManager
    {
        public static UnityEngine.MonoBehaviour MonoBehaviourInstance { get; private set; }
        private static readonly ListEx<TaskEntity> TaskList_ = new ListEx<TaskEntity>();
        private static readonly object MainThreadLock_ = new object();
        private static readonly ListEx<MainThreadTaskEntity> MainThreadTaskList_ = new ListEx<MainThreadTaskEntity>();

        public static bool Startup(UnityEngine.MonoBehaviour Instance)
        {
            MonoBehaviourInstance = Instance;
            TaskList_.Clear();
            MainThreadTaskList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            MonoBehaviourInstance.StopAllCoroutines();
            foreach (var Entity in TaskList_)
            {
                Entity.Dispose();
            }
            TaskList_.Clear();

            lock (MainThreadLock_)
            {
                MainThreadTaskList_.Clear();
            }
        }

        public static void Tick(float DeltaTime)
        {
            TaskList_.Foreach((Entity) =>
            {
                if (Entity.IsEnd)
                {
                    Entity.Dispose();
                    TaskList_.Remove(Entity);
                }
            });

            if (MainThreadTaskList_.RealCount > 0)
            {
                lock (MainThreadLock_)
                {
                    MainThreadTaskList_.Foreach((Entity) => { Entity?.Invoke(); });
                    MainThreadTaskList_.Clear();
                }
            }
        }

        public static TaskEntity AddTask(IEnumerator TaskFunc, Action Callback = null)
        {
            var NewTask = new TaskEntity(TaskFunc, Callback);
            TaskList_.Add(NewTask);
            MonoBehaviourInstance.StartCoroutine(NewTask.Execute());
            return NewTask;
        }

        public static IEnumerator WaitTask(IEnumerator TaskFunc, Action Callback = null)
        {
            var NewTask = new TaskEntity(TaskFunc, Callback);
            TaskList_.Add(NewTask);
            yield return MonoBehaviourInstance.StartCoroutine(NewTask.Execute());
        }

        public static void AddMainThreadTask(Action<object> TaskFunc, object Param)
        {
            lock (MainThreadLock_)
            {
                MainThreadTaskList_.Add(new MainThreadTaskEntity(TaskFunc, Param));
            }
        }
    }
}