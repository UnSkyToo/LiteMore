using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc
    {
        public BaseNpc AttackerNpc { get; set; }
        public BaseNpc TargetNpc { get; set; }
        public Vector2 TargetPos { get; protected set; }
        public bool IsForceMove { get; protected set; }

        protected List<BaseSkill> SkillList_ = new List<BaseSkill>();

        public void Back(float Distance, float Speed)
        {
            Back((Position - TargetPos).normalized * Distance, Speed);
        }

        public void Back(Vector2 Offset, float Speed)
        {
            if (IsStatic)
            {
                return;
            }

            var BackPos = Position + Offset;
            EventManager.Send(new NpcBackEvent(ID, Team, BackPos, Offset.magnitude / Speed));
        }

        public void StopMove()
        {
            Fsm_.ChangeToIdleState();
            IsForceMove = false;
        }

        public void MoveTo(Vector2 TargetPos, bool IsForceMove = false)
        {
            if (IsStatic)
            {
                return;
            }

            this.TargetPos = TargetPos;
            this.IsForceMove = IsForceMove;
            EventManager.Send(new NpcWalkEvent(ID, Team, TargetPos));
        }

        public bool CanUseSkill()
        {
            if (!IsAlive || IsForceMove)
            {
                return false;
            }

            if (IsState(NpcState.Dizzy))
            {
                return false;
            }

            if (IsFsmState(FsmStateName.Idle) || IsFsmState(FsmStateName.Walk))
            {
                return true;
            }

            return false;
        }

        public bool CanUseSkill(uint SkillID)
        {
            if (!CanUseSkill())
            {
                return false;
            }

            var Skill = GetSkill(SkillID);
            if (Skill == null)
            {
                return false;
            }

            return Skill.CanUse();
        }

        public uint GetSkillLevel(uint SkillID)
        {
            return 1;
        }

        public List<BaseSkill> GetSkillList()
        {
            return SkillList_;
        }

        public void AddSkill(BaseSkill Skill)
        {
            SkillList_.Add(Skill);
        }

        public BaseSkill GetSkill(uint SkillID)
        {
            foreach (var Skill in SkillList_)
            {
                if (Skill.SkillID == SkillID)
                {
                    return Skill;
                }
            }

            return null;
        }

        public void UseSkill(uint SkillID)
        {
            if (!CanUseSkill(SkillID))
            {
                return;
            }

            var SkillDesc = SkillLibrary.Get(SkillID);
            if (SkillDesc == null)
            {
                return;
            }

            object Args = null;
            if (SkillDesc.Rule.RangeType == LockRangeType.InDistance)
            {
                Args = SkillDesc.Radius;
            }

            var Targets = LockingHelper.Find(this, SkillDesc.Rule, Args);
            if (Targets.Count == 0)
            {
                return;
            }

            var Evt = new NpcSkillEvent(ID, Team, SkillID, Targets);
            EventManager.Send(Evt);
        }

        public bool IsValidAttacker()
        {
            return AttackerNpc != null && AttackerNpc.IsValid();
        }

        public bool IsValidTarget()
        {
            return TargetNpc != null && TargetNpc.IsValid();
        }

        public void ForceTarget(BaseNpc Target)
        {
            TargetNpc = Target;
        }
    }
}