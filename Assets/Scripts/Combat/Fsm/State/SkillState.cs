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
            Skill_ = Fsm.Master.Skill.GetSkill(SkillID_) as NpcSkill;
            if (Skill_ == null)
            {
                Fsm.ChangeToIdleState();
                return;
            }

            Args_ = Evt.Args ?? Fsm.Master.Skill.CreateSkillArgs(SkillID_);

            if (Fsm.Master.Actor.HasAnimation("Attack"))
            {
                Fsm.Master.Actor.PlayAnimation("Attack", false);

                if (!Mathf.Approximately(0, Args_.Direction.sqrMagnitude))
                {
                    Fsm.Master.Actor.FaceToPosition(Fsm.Master.Position + Args_.Direction);
                }
                else if (!Mathf.Approximately(0, Args_.Position.sqrMagnitude))
                {
                    Fsm.Master.Actor.FaceToPosition(Args_.Position);
                }
                else if (Fsm.Master.Action.IsValidTarget())
                {
                    Fsm.Master.Actor.FaceToPosition(Fsm.Master.Action.TargetNpc.Position);
                }
            }
            else
            {
                OnAtkMsg();
                Fsm.ChangeToIdleState();
            }
        }

        public override void OnTick(float DeltaTime)
        {
            if (Fsm.Master.Actor.AnimationIsEnd())
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
                if (SkillID_ != 1001)
                {
                    LabelManager.AddStringLabel(Fsm.Master.Position, Skill_.Name, Color.green, 30);
                }
            }
        }
    }
}