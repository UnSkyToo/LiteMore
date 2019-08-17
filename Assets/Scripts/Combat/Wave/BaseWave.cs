using LiteFramework.Core.Event;
using LiteMore.Combat.Emitter;
using LiteMore.Core;
using LiteMore.Data;
using LiteMore.UI;
using UnityEngine;

namespace LiteMore.Combat.Wave
{
    public class BaseWave : BaseEntity
    {
        public uint Wave { get; }
        public WaveData Data { get; }

        protected BaseEmitter Emitter_;
        protected uint RemainingCount_;

        public BaseWave(uint Wave)
            : base($"Wave{Wave}")
        {
            this.Wave = Wave;

            Data = LocalData.WaveList[Wave];
            Emitter_ = EmitterManager.AddEmitter(new NpcRectEmitter(Name)
            {
                Team = CombatTeam.B,
                TriggerCount = -1,//Data.TriggerCount,
                EmittedCountAttr = new EmitterRandInt(1, 5),//Data.EmitterCount,
                Interval = 1f,//Data.Interval,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 120, 0),
                SpeedAttr = new EmitterRandFloat(Data.Speed * 0.8f, Data.Speed * 1.2f),
                HpAttr = new EmitterRandFloat(Data.Hp),
                DamageAttr = new EmitterRandFloat(Data.Damage),
                GemAttr = new EmitterRandInt(Data.Gem),
                OffsetAttr = new EmitterRandVector2(
                    new Vector2(-100, -Configure.WindowHeight / 2 + 100),
                    new Vector2(100, Configure.WindowHeight / 2 - 100)),
            });
            Emitter_.OnTrigger += (Count) => RemainingCount_ += Count;
            //Emitter_.AtOnce();

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
            if (Event.MasterTeam == CombatTeam.B)
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
    }
}