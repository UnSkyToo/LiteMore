using System;
using System.Threading;

namespace LiteFramework.Core.Async.Job
{
    public abstract class JobSystem<TEntity>
    {
        private readonly TEntity[] Entities_;
        private readonly Job<TEntity>[] Jobs_;
        private readonly int ThreadCount_;
        private int ThreadRunningCount_;

        public event Action OnCompleted;

        protected JobSystem(TEntity[] Entities, int ThreadCount)
        {
            Entities_ = Entities;
            ThreadCount_ = ThreadCount;
            var Remainder = Entities_.Length % ThreadCount_;
            var Slice = Entities_.Length / ThreadCount_ + (Remainder == 0 ? 0 : 1);

            Jobs_ = new Job<TEntity>[ThreadCount];
            for (var Index = 0; Index < ThreadCount; ++Index)
            {
                var From = Index * Slice;
                var To = From + Slice;

                if (To > Entities_.Length)
                {
                    To = Entities_.Length;
                }

                Jobs_[Index] = new Job<TEntity>();
                Jobs_[Index].Set(Entities_, From, To);
            }
        }

        public void Execute()
        {
            ThreadRunningCount_ = ThreadCount_;
            for (var Index = 0; Index < ThreadCount_; ++Index)
            {
                var JobTask = Jobs_[Index];

                if (Jobs_[Index].From != Jobs_[Index].To)
                {
                    ThreadPool.QueueUserWorkItem(QueueOnThread, JobTask);
                }
                else
                {
                    OnTaskDone();
                }
            }
        }

        private void QueueOnThread(object State)
        {
            var JobTask = State as Job<TEntity>;

            try
            {
                for (var Index = JobTask.From; Index < JobTask.To; ++Index)
                {
                    OnExecute(JobTask.Entities[Index]);
                }
            }
            catch (LiteException Ex)
            {
                JobTask.Ex = Ex;
            }
            finally
            {
                OnTaskDone();
            }
        }

        private void OnTaskDone()
        {
            Interlocked.Decrement(ref ThreadRunningCount_);

            if (ThreadRunningCount_ == 0)
            {
                foreach (var JobTask in Jobs_)
                {
                    if (JobTask.Ex != null)
                    {
                        throw JobTask.Ex;
                    }
                }

                OnCompleted?.Invoke();
            }
        }

        protected abstract void OnExecute(TEntity Entity);
    }
}