using System;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Timer
{
    public class TimerEntity : BaseObject, IDisposable
    {
        public event Action OnTick;
        public event Action OnEnd;

        public bool IsEnd => Count_ == 0;

        private readonly float Interval_;
        private float Time_;
        private int Count_;
        private bool IsPause_;

        public TimerEntity(float Interval, int Count)
            : base()
        {
            Interval_ = Interval;
            Time_ = 0.0f;
            Count_ = Count;
            IsPause_ = false;
        }

        public void Dispose()
        {
            OnTick = null;
            OnEnd = null;
        }

        public void Start()
        {
            IsPause_ = false;
        }

        public void Pause()
        {
            IsPause_ = true;
        }

        public void Stop()
        {
            Pause();
            Count_ = 0;
            TriggerEndEvent();
        }

        public void Tick(float DeltaTime)
        {
            if (IsPause_)
            {
                return;
            }

            Time_ += DeltaTime;

            if (Time_ >= Interval_)
            {
                Time_ -= Interval_;
                TriggerTickEvent();
            }
        }

        private void TriggerTickEvent()
        {
            OnTick?.Invoke();

            if (Count_ > 0)
            {
                Count_--;

                if (Count_ == 0)
                {
                    Stop();
                }
            }
        }

        private void TriggerEndEvent()
        {
            OnEnd?.Invoke();
        }
    }
}