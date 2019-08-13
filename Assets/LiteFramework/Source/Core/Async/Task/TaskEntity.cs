using System;
using System.Collections;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Task
{
    public class TaskEntity : BaseObject
    {
        public bool IsPause { get; set; }
        public bool IsEnd { get; private set; }

        private readonly IEnumerator TaskEntity_;
        private readonly Action Callback_;

        public TaskEntity(IEnumerator Entity, Action Callback)
            : base()
        {
            IsPause = false;
            IsEnd = false;

            TaskEntity_ = Entity;
            Callback_ = Callback;
        }

        public void Start()
        {
            IsPause = false;
        }

        public void Pause()
        {
            IsPause = true;
        }

        public void Stop()
        {
            Pause();
            IsEnd = true;
        }

        public IEnumerator Execute()
        {
            while (!IsEnd)
            {
                if (IsPause)
                {
                    yield return null;
                }
                else if (TaskEntity_ != null && TaskEntity_.MoveNext())
                {
                    yield return TaskEntity_.Current;
                }
                else
                {
                    IsEnd = true;
                }
            }

            Callback_?.Invoke();
        }
    }
}