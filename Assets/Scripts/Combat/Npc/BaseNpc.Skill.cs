using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc
    {
        public BaseNpc AttackerNpc { get; set; }
        public BaseNpc TargetNpc { get; set; }
        public Vector2 TargetPos { get; protected set; }

        protected List<BaseSkill> SkillList_ = new List<BaseSkill>();
        protected BaseSkill NextSkill_ = null;

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

        public void MoveTo(Vector2 TargetPos)
        {
            if (IsStatic)
            {
                return;
            }

            this.TargetPos = TargetPos;
            EventManager.Send(new NpcWalkEvent(ID, Team, TargetPos));
        }

        public bool CanUseSkill()
        {
            if (!IsAlive)
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

        public void SetNextSkill(BaseSkill Skill)
        {
            NextSkill_ = Skill;
        }

        public BaseSkill GetNextSkill()
        {
            return NextSkill_;
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

        public void UseNextSkill()
        {
            var Evt = new NpcSkillEvent(ID, Team, TargetNpc.ID, NextSkill_.SkillID);
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