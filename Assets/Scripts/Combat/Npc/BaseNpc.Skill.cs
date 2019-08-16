using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc
    {
        protected List<BaseSkill> SkillList_ = new List<BaseSkill>();
        protected BaseSkill NextSkill_ = null;

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
    }
}