using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Emitter
{
    public abstract class BaseEmitter : BaseEntity
    {
        public Vector2 Position { get; set; }
        public CombatTeam Team { get; set; }
        public bool IsPause { get; set; }
        public float Interval { get; set; }
        public uint EmittedCount { get; set; }
        public int TriggerCount { get; set; }

        private float Time_;

        protected BaseEmitter()
            : base()
        {
            Position = Vector2.zero;
            Team = CombatTeam.B;
            IsPause = false;
            Interval = 1;
            Time_ = 0;
            EmittedCount = 1;
            TriggerCount = -1;
        }

        public override void Dispose()
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

        public void AtOnce()
        {
            Time_ = Interval;
        }

        public uint GetRemainingCount()
        {
            if (TriggerCount > 0)
            {
                return (uint)(TriggerCount * EmittedCount);
            }

            return 0;
        }

        public abstract void CreateDebugLine();

        protected abstract void OnEmitted(uint Cur, uint Max);
    }
}