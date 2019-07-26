using LiteMore.Combat.Emitter;
using LiteMore.UI;
using UnityEngine;

namespace LiteMore.Combat.Wave
{
    public class BaseWave
    {
        public uint Wave { get; }
        public bool IsEnd { get; protected set; }

        protected BaseEmitter Emitter_;
        protected uint RemainingCount_;

        public BaseWave(uint Wave)
        {
            this.Wave = Wave;
            this.IsEnd = false;

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
            RemainingCount_ = Emitter_.GetRemainingCount();
            //Emitter_.AtOnce();

            EventManager.Register<NpcDieEvent>(OnNpcDieEvent);
        }

        public void Destroy()
        {
            EventManager.UnRegister<NpcDieEvent>(OnNpcDieEvent);
        }

        public void Tick(float DeltaTime)
        {
            IsEnd = (RemainingCount_ == 0);
        }

        private void OnNpcDieEvent(NpcDieEvent Event)
        {
            if (Event.Master.Team == CombatTeam.B)
            {
                RemainingCount_--;

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