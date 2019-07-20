namespace LiteMore.Combat.Emitter
{
    public abstract class EmitterBase : EntityBase
    {
        public bool IsPause { get; set; }
        public float Interval { get; set; }
        public uint EmittedCount { get; set; }
        public int TriggerCount { get; set; }

        private float Time_;

        protected EmitterBase()
            : base()
        {
            IsPause = false;
            Interval = 1;
            Time_ = 0;
            EmittedCount = 1;
            TriggerCount = -1;
        }

        public override void Create()
        {
        }

        public override void Destroy()
        {
        }

        public override void Tick(float DeltaTime)
        {
            if (IsPause || !IsAlive)
            {
                return;
            }

            Time_ += DeltaTime;
            if (Time_ >= Interval)
            {
                Time_ -= Interval;

                for (var Index = 0u; Index < EmittedCount; ++Index)
                {
                    OnEmitted(Index, EmittedCount);
                }

                if (TriggerCount > 0)
                {
                    TriggerCount--;
                    if (TriggerCount == 0)
                    {
                        IsPause = true;
                        IsAlive = false;
                    }
                }
            }
        }

        protected abstract void OnEmitted(uint Cur, uint Max);
    }
}