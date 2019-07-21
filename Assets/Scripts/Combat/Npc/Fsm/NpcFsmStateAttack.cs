﻿namespace LiteMore.Combat.Npc.Fsm
{
    public class NpcFsmStateAttack : NpcFsmStateBase
    {
        public NpcFsmStateAttack(NpcFsm Fsm)
            : base(NpcFsmStateName.Attack, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);

            PlayerManager.AddHp(-Fsm.Master.Damage);
            Fsm.ChangeToState(NpcFsmStateName.Die, new DieEvent(Fsm.Master.ID));
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is DieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }
}