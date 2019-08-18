using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Fsm.State
{
    public class SkillState : BaseState
    {
        private uint SkillID_;
        private List<BaseNpc> TargetList_;

        public SkillState(BaseFsm Fsm)
            : base(FsmStateName.Skill, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            var Evt = Event as NpcSkillEvent;
            SkillID_ = Evt.SkillID;
            TargetList_ = Evt.TargetList;

            Fsm.Master.PlayAnimation("Attack", false);
            Fsm.Master.TurnToTarget();
        }

        public override void OnTick(float DeltaTime)
        {
            if (Fsm.Master.AnimationIsEnd())
            {
                Fsm.ChangeToIdleState();
                return;
            }
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
                var Evt = new NpcHitTargetEvent(Fsm.Master.ID, Fsm.Master.Team, SkillID_, TargetList_, "prefabs/sfx/hitsfx.prefab");
                EventManager.Send(Evt);
            }
        }
    }
}