using LiteMore.Combat.Label;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.Fsm.State
{
    public class SkillState : BaseState
    {
        private uint SkillID_;
        private NpcSkill Skill_;
        private SkillArgs Args_;

        public SkillState(BaseFsm Fsm)
            : base(FsmStateName.Skill, Fsm)
        {
        }

        public override void OnEnter(CombatEvent Event)
        {
            var Evt = Event as NpcSkillEvent;
            SkillID_ = Evt.SkillID;
            Skill_ = Fsm.Master.GetSkill(SkillID_) as NpcSkill;
            if (Skill_ == null)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            Args_ = Evt.Args ?? Fsm.Master.CreateSkillArgs(SkillID_);

            if (Fsm.Master.HasAnimation("Attack"))
            {
                Fsm.Master.PlayAnimation("Attack", false);
                Fsm.Master.TurnToTarget();
            }
            else
            {
                OnAtkMsg();
                Fsm.ChangeToIdleState();
            }
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
        }

        public override void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            if (MsgCode == CombatMsgCode.Atk)
            {
                OnAtkMsg();
            }
        }

        private void OnAtkMsg()
        {
            Args_.HitSfx = "prefabs/sfx/hitsfx.prefab";
            Args_.LockRule = Skill_.Rule;

            if (Skill_.Use(Args_))
            {
                // temp code : show skill name (ignore normal attack)
                if (SkillID_ != 3001)
                {
                    LabelManager.AddStringLabel(Fsm.Master.Position, Skill_.Name, Color.green, 30);
                }
            }
        }
    }
}