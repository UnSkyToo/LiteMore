using System;
using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Emitter
{
    public abstract class BaseEmitter : BaseEntity
    {
        public event Action<uint> OnTrigger; 

        public Vector2 Position { get; set; }
        public CombatTeam Team { get; set; }
        public bool IsPause { get; set; }
        public float Interval { get; set; }
        public EmitterRandInt EmittedCountAttr { get; set; }
        public int TriggerCount { get; set; }

        private float Time_;

        protected BaseEmitter(string Name)
            : base(Name)
        {
            Position = Vector2.zero;
            Team = CombatTeam.B;
            IsPause = false;
            Interval = 1;
            Time_ = 0;
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

                var Count = EmittedCountAttr.Get();
                Count = Mathf.Clamp(Count, 0, int.MaxValue);
                for (var Index = 0u; Index < Count; ++Index)
                {
                    OnEmitted(Index, (uint)Count);
                }
                OnTrigger?.Invoke((uint)Count);

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

        public abstract void CreateDebugLine();

        protected abstract void OnEmitted(uint Cur, uint Max);
    }
}