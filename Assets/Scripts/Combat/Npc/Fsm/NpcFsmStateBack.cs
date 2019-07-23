using UnityEngine;

namespace LiteMore.Combat.Npc.Fsm
{
    public class NpcFsmStateBack : NpcFsmStateBase
    {
        private Vector2 StartPos_;
        private Vector2 BackPos_;
        private float BackTime_;
        private float Time_;
        private bool IsEnd_;

        public NpcFsmStateBack(NpcFsm Fsm)
            : base(NpcFsmStateName.Back, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);
            var Evt = (Event as NpcBackEvent);

            StartPos_ = Fsm.Master.Position;
            BackPos_ = Evt.BackPos;
            BackTime_ = Evt.BackTime;
            Time_ = 0;
            IsEnd_ = false;
        }

        public override void OnTick(float DeltaTime)
        {
            if (IsEnd_)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            Time_ += DeltaTime;
            var T = Time_ / BackTime_;
            if (T >= 1.0f)
            {
                IsEnd_ = true;
                T = 1.0f;
            }

            Fsm.Master.Position = Vector2.Lerp(StartPos_, BackPos_, T);
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is NpcDieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }
}