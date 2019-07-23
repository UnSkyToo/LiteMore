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
            if (Event is NpcMoveEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Walk, Event);
            }
            else if (Event is NpcDieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
            else if (Event is NpcBackEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Back, Event);
            }
        }
    }
}