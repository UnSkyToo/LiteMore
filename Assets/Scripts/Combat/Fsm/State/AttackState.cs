namespace LiteMore.Combat.Fsm.State
{
    public class AttackState : BaseState
    {
        public AttackState(BaseFsm Fsm)
            : base(FsmStateName.Attack, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            Fsm.Master.PlayAnimation("Attack", true);

            //Fsm.ChangeToState(NpcFsmStateName.Die, new NpcDieEvent(Fsm.Master.ID));
        }

        public override void OnCombatEvent(CombatEvent Event)
        {
            if (Event is NpcDieEvent)
            {
                Fsm.ChangeToState(FsmStateName.Die, Event);
            }
            else if (Event is NpcIdleEvent)
            {
                Fsm.ChangeToIdleState();
            }
            else if (Event is NpcWalkEvent)
            {
                Fsm.ChangeToState(FsmStateName.Walk, Event);
            }
        }

        public override void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            if (MsgCode == CombatMsgCode.Atk)
            {
                //PlayerManager.AddHp(-(Fsm.Master.CalcFinalAttr(NpcAttrIndex.Damage)));

                if (Fsm.Master.IsValidTarget())
                {
                    Fsm.Master.TargetNpc.OnNpcHit(Fsm.Master);
                }
            }
        }
    }
}