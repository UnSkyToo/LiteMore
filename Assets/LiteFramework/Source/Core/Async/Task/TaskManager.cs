using System;
using System.Collections;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Task
{
    public static class TaskManager
    {
        public static UnityEngine.MonoBehaviour MonoBehaviourInstance { get; private set; }
        private static readonly ListEx<TaskEntity> TaskList_ = new ListEx<TaskEntity>();

        public static bool Startup(UnityEngine.MonoBehaviour Instance)
        {
            MonoBehaviourInstance = Instance;
            TaskList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            MonoBehaviourInstance.StopAllCoroutines();
            TaskList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            foreach (var Entity in TaskList_)
            {
                if (Entity.IsEnd)
                {
                    TaskList_.Remove(Entity);
                }
            }
            TaskList_.Flush();
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
    }
}