using System;
using System.Collections.Generic;

namespace LiteMore
{
    public class Timer
    {
        public event Action OnTick;
        public event Action OnEnd;

        public uint ID { get; }
        public bool IsEnd => Count_ == 0;

        private float Interval_;
        private float Time_;
        private int Count_;
        private bool IsPause_;

        public Timer(float Interval, int Count)
        {
            ID = IDGenerator.Get();
            Interval_ = Interval;
            Time_ = 0.0f;
            Count_ = Count;
            IsPause_ = false;
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

    public static class TimerManager
    {
        private static readonly Dictionary<uint, Timer> TimerList_ = new Dictionary<uint, Timer>();
        private static readonly List<Timer> AddList_ = new List<Timer>();
        private static readonly List<uint> RemoveList_ = new List<uint>();

        public static bool Startup()
        {
            TimerList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            TimerList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            if (AddList_.Count > 0)
            {
                foreach (var Entity in AddList_)
                {
                    TimerList_.Add(Entity.ID, Entity);
                }

                AddList_.Clear();
            }

            foreach (var Entity in TimerList_)
            {
                Entity.Value.Tick(DeltaTime);

                if (Entity.Value.IsEnd)
                {
                    RemoveList_.Add(Entity.Key);
                }
            }

            if (RemoveList_.Count > 0)
            {
                foreach (var ID in RemoveList_)
                {
                    TimerList_.Remove(ID);
                }
            }
        }

        public static uint AddTimer(float Interval, Action OnTick, int Count = -1)
        {
            var NewTimer = new Timer(Interval, Count);
            NewTimer.OnTick += OnTick;
            AddList_.Add(NewTimer);
            return NewTimer.ID;
        }

        public static uint AddTimer(float Interval, Action OnTick, Action OnEnd, int Count = -1)
        {
            var NewTimer = new Timer(Interval, Count);
            NewTimer.OnTick += OnTick;
            NewTimer.OnEnd += OnEnd;
            AddList_.Add(NewTimer);
            return NewTimer.ID;
        }

        public static void StopTimer(uint ID)
        {
            if (TimerList_.ContainsKey(ID))
            {
                TimerList_[ID].Stop();
            }
        }

        public static void StartTimer(uint ID)
        {
            if (TimerList_.ContainsKey(ID))
            {
                TimerList_[ID].Start();
            }
        }

        public static void PauseTimer(uint ID)
        {
            if (TimerList_.ContainsKey(ID))
            {
                TimerList_[ID].Pause();
            }
        }
    }
}