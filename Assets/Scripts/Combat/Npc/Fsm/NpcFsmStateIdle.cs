namespace LiteMore.Combat.Npc.Fsm
{
    public class NpcFsmStateIdle : NpcFsmStateBase
    {
        public NpcFsmStateIdle(NpcFsm Fsm)
            : base(NpcFsmStateName.Idle, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is MoveEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Walk, Event);
            }
            else if (Event is DieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }
}