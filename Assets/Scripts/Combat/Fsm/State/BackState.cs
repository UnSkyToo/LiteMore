using UnityEngine;

namespace LiteMore.Combat.Fsm.State
{
    public class BackState : BaseState
    {
        private Vector2 StartPos_;
        private Vector2 BackPos_;
        private float BackTime_;
        private float Time_;
        private bool IsEnd_;

        public BackState(BaseFsm Fsm)
            : base(FsmStateName.Back, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            if (Fsm.Master.IsStatic)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            Fsm.Master.Actor.PlayAnimation("Idle", true);
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

        public override void OnCombatEvent(CombatEvent Event)
        {
            if (Event is NpcDieEvent)
            {
                Fsm.ChangeToState(FsmStateName.Die, Event);
            }
        }
    }
}