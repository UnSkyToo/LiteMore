using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
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

        protected BaseSkill AddSkill(BaseSkill Skill, bool Notify = true)
        {
            if (Skill == null)
            {
                return null;
            }

            SkillList_.Add(Skill);

            if (Notify)
            {
                EventManager.Send(new NpcSkillChangedEvent(ID, Team, Skill.SkillID, true));
            }

            return Skill;
        }

        public BaseSkill RemoveSkill(uint SkillID, bool Notify = true)
        {
            var Skill = GetSkill(SkillID);
            if (Skill == null)
            {
                return null;
            }

            if (Notify)
            {
                EventManager.Send(new NpcSkillChangedEvent(ID, Team, SkillID, false));
            }

            SkillManager.RemoveSkill(Skill);
            SkillList_.Remove(Skill);
            return Skill;
        }

        public NpcSkill AddNpcSkill(uint SkillID, bool Notify = true)
        {
            var Skill = SkillManager.AddNpcSkill(SkillLibrary.Get(SkillID), this);
            if (Skill == null)
            {
                return null;
            }

            return AddSkill(Skill, Notify) as NpcSkill;
        }

        public PassiveSkill AddPassiveSkill(uint SkillID, float SustainTime, bool Notify = true)
        {
            var Skill = SkillManager.AddPassiveSkill(SkillLibrary.Get(SkillID), this, SustainTime);
            if (Skill == null)
            {
                return null;
            }

            return AddSkill(Skill, Notify) as PassiveSkill;
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

        public SkillArgs CreateSkillArgs(uint SkillID)
        {
            return new SkillArgs(GetSkill(SkillID));
        }

        public void UseSkill(SkillArgs Args)
        {
            if (Args == null || Args.Skill == null)
            {
                LLogger.LWarning("Npc UseSkill, Args or Args.Skill is null");
                return;
            }

            if (!CanUseSkill(Args.Skill.SkillID))
            {
                return;
            }

            var Evt = new NpcSkillEvent(ID, Team, Args.Skill.SkillID, Args);
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