using LiteMore.Combat.Emitter;
using LiteMore.Core;
using LiteMore.UI;
using UnityEngine;

namespace LiteMore.Combat.Wave
{
    public class BaseWave : BaseEntity
    {
        public uint Wave { get; }

        protected BaseEmitter Emitter_;
        protected uint RemainingCount_;

        public BaseWave(uint Wave)
        {
            this.Wave = Wave;

            Emitter_ = EmitterManager.AddEmitter(new NpcRectEmitter
            {
                Team = CombatTeam.B,
                TriggerCount = CalcTriggerCount(),
                EmittedCount = CalcEmittedCount(),
                Interval = 240.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, 0),
                SpeedAttr = new EmitterRandFloat(50, 100),
                HpAttr = new EmitterRandInt(5, 10),
                DamageAttr = new EmitterRandInt(1, 1),
                GemAttr = new EmitterRandInt(1, 1),
                OffsetAttr = new EmitterRandVector2(
                    new Vector2(-100, -Configure.WindowHeight / 2 + 100),
                    new Vector2(100, Configure.WindowHeight / 2 - 100)),
            });
            //Emitter_.AtOnce();
            RemainingCount_ = Emitter_.GetRemainingCount();

            EventManager.Register<NpcDieEvent>(OnNpcDieEvent);
        }

        public override void Dispose()
        {
            EventManager.UnRegister<NpcDieEvent>(OnNpcDieEvent);
        }

        public override void Tick(float DeltaTime)
        {
        }

        private void OnNpcDieEvent(NpcDieEvent Event)
        {
            if (Event.Master.Team == CombatTeam.B)
            {
                if (RemainingCount_ > 0)
                {
                    RemainingCount_--;

                    if (RemainingCount_ == 0)
                    {
                        IsAlive = false;
                    }
                }

                EventManager.Send<WaveChangeEvent>();
            }
        }

        public uint GetRemainingCount()
        {
            return RemainingCount_;
        }

        private int CalcTriggerCount()
        {
            return (int)(4 + (Wave / 3));
        }

        private uint CalcEmittedCount()
        {
            return 4 + Wave;
        }

        private float CalcInterval()
        {
            return 4;
        }
    }
}