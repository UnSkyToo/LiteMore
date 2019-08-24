using LiteMore.Core;

namespace LiteMore.Combat.Buff
{
    public abstract class BaseBuffDescriptor
    {
        public string Name { get; }
        public float Duration { get; }
        public float Interval { get; }
        public float WaitTime { get; }
        public bool IsRefund { get; }

        protected BaseBuffDescriptor(string Name, float Duration, float Interval, float WaitTime, bool IsRefund)
        {
            this.Name = Name;
            this.Duration = Duration;
            this.Interval = Interval;
            this.WaitTime = WaitTime;
            this.IsRefund = IsRefund;
        }
    }

    public abstract class BaseBuff : BaseEntity
    {
        public BuffType Type { get; }

        private bool IsWait_;
        private float WaitTime_;
        private float DurationTime_;
        private readonly float Interval_;
        private float IntervalTime_;
        protected readonly bool IsRefund_;

        protected BaseBuff(BuffType Type, BaseBuffDescriptor Desc)
            : base(Desc.Name)
        {
            this.Type = Type;
            this.DurationTime_ = Desc.Duration;
            this.IsWait_ = Desc.WaitTime > 0;
            this.WaitTime_ = Desc.WaitTime;
            this.Interval_ = Desc.Interval;
            this.IntervalTime_ = 0;
            this.IsRefund_ = Desc.IsRefund;
        }

        public override void Tick(float DeltaTime)
        {
            if (IsWait_)
            {
                WaitTime_ -= DeltaTime;

                if (WaitTime_ <= 0)
                {
                    IsWait_ = false;
                    Attach();
                }

                return;
            }

            DurationTime_ -= DeltaTime;
            if (DurationTime_ <= 0)
            {
                IsAlive = false;
                return;
            }

            if (Interval_ > 0)
            {
                IntervalTime_ += DeltaTime;

                if (IntervalTime_ >= Interval_)
                {
                    IntervalTime_ -= Interval_;
                    Trigger();
                }
            }
        }

        public override void Dispose()
        {
            Detach();
        }

        public void TryAttach()
        {
            if (!IsWait_)
            {
                Attach();
            }
        }

        public void Attach()
        {
            OnAttach();
            Trigger();
        }

        protected abstract void OnAttach();

        public void Detach()
        {
            OnDetach();
        }

        protected abstract void OnDetach();

        public void Trigger()
        {
            OnTrigger();
        }

        protected abstract void OnTrigger();
    }
}