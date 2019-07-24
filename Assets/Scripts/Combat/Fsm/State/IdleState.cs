namespace LiteMore.Combat.Fsm.State
{
    public class IdleState : BaseState
    {
        public IdleState(BaseFsm Fsm)
            : base(FsmStateName.Idle, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);
        }

        public override void OnCombatEvent(CombatEvent Event)
        {
            if (Event is NpcWalkEvent)
            {
                Fsm.ChangeToState(FsmStateName.Walk, Event);
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