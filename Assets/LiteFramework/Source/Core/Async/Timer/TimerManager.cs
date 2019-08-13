using System;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Timer
{
    public static class TimerManager
    {
        private static readonly ListEx<TimerEntity> TimerList_ = new ListEx<TimerEntity>();

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
            foreach (var Entity in TimerList_)
            {
                Entity.Tick(DeltaTime);

                if (Entity.IsEnd)
                {
                    TimerList_.Remove(Entity);
                }
            }
            TimerList_.Flush();
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, int Count = -1)
        {
            var NewTimer = new TimerEntity(Interval, Count);
            NewTimer.OnTick += OnTick;
            return NewTimer;
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, Action OnEnd, int Count = -1)
        {
            var NewTimer = new TimerEntity(Interval, Count);
            NewTimer.OnTick += OnTick;
            NewTimer.OnEnd += OnEnd;
            return NewTimer;
        }

        public static void StopTimer(TimerEntity Entity)
        {
            Entity?.Stop();
        }
    }
}