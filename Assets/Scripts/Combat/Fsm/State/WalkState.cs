using UnityEngine;

namespace LiteMore.Combat.Fsm.State
{
    public class WalkState : BaseState
    {
        private Vector2 BeginPos_;
        private Vector2 EndPos_;
        private float MoveTime_;
        private float MoveTotalTime_;
        private bool IsMove_;

        public WalkState(BaseFsm Fsm)
            : base(FsmStateName.Walk, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            if (Fsm.Master.IsStatic)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            BeginPos_ = Fsm.Master.Position;
            EndPos_ = (Event as NpcWalkEvent).TargetPos;
            MoveTime_ = 0;
            MoveTotalTime_ = (EndPos_ - BeginPos_).magnitude / Fsm.Master.CalcFinalAttr(NpcAttrIndex.Speed);
            IsMove_ = true;

            Fsm.Master.PlayAnimation("Walk", true);
            Fsm.Master.SetDirection(CombatHelper.CalcDirection(BeginPos_, EndPos_));
        }

        public override void OnTick(float DeltaTime)
        {
            if (!IsMove_)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            MoveTime_ += DeltaTime;
            var T = MoveTime_ / MoveTotalTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                IsMove_ = false;
            }

            Fsm.Master.Position = Vector2.Lerp(BeginPos_, EndPos_, T);
        }

        public override void OnCombatEvent(CombatEvent Event)
        {
            if (Event is NpcWalkEvent)
            {
                Fsm.ChangeToState(FsmStateName.Walk, Event);
            }
            else if (Event is NpcIdleEvent)
            {
                Fsm.ChangeToIdleState();
            }
            else if (Event is NpcSkillEvent)
            {
                Fsm.ChangeToState(FsmStateName.Skill, Event);
            }
            else if (Event is NpcDieEvent)
            {
                Fsm.ChangeToState(FsmStateName.Die, Event);
            }
            else if (Event is NpcBackEvent)
            {
                Fsm.ChangeToState(FsmStateName.Back, Event);
            }
        }
    }
}