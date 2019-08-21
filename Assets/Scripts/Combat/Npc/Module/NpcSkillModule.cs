using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.Npc.Module
{
    public class NpcSkillModule : BaseNpcModule
    {
        protected List<BaseSkill> SkillList_ = new List<BaseSkill>();

        public NpcSkillModule(BaseNpc Master)
            : base(Master)
        {
        }

        public bool CanUseSkill()
        {
            if (!Master.IsAlive)
            {
                return false;
            }

            if (Master.Action.IsForceMove)
            {
                return false;
            }

            if (Master.Data.IsState(NpcState.Dizzy))
            {
                return false;
            }

            if (Master.Actor.IsFsmState(FsmStateName.Idle) || Master.Actor.IsFsmState(FsmStateName.Walk))
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

        protected BaseSkill AddSkill(BaseSkill Skill, bool Notify)
        {
            if (Skill == null)
            {
                return null;
            }

            SkillList_.Add(Skill);
            SkillList_.Sort((A, B) =>
            {
                if (A.Priority > B.Priority)
                {
                    return -1;
                }

                if (A.Priority < B.Priority)
                {
                    return 1;
                }

                return 0;
            });

            if (Notify)
            {
                EventManager.Send(new NpcSkillChangedEvent(Master, Skill.SkillID, true));
            }

            return Skill;
        }

        public void RemoveSkill(uint SkillID, bool Notify = false)
        {
            var Skill = GetSkill(SkillID);
            if (Skill == null)
            {
                return;
            }

            if (Notify)
            {
                EventManager.Send(new NpcSkillChangedEvent(Master, SkillID, false));
            }

            SkillManager.RemoveSkill(Skill);
            SkillList_.Remove(Skill);
        }

        public NpcSkill AddNpcSkill(uint SkillID, bool Notify = false)
        {
            var Skill = SkillManager.AddNpcSkill(SkillLibrary.Get(SkillID), Master);
            if (Skill == null)
            {
                return null;
            }

            return AddSkill(Skill, Notify) as NpcSkill;
        }

        public PassiveSkill AddPassiveSkill(uint SkillID, float SustainTime, bool Notify = false)
        {
            var Skill = SkillManager.AddPassiveSkill(SkillLibrary.Get(SkillID), Master, SustainTime);
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

            var Evt = new NpcSkillEvent(Master, Args.Skill.SkillID, Args);
            EventManager.Send(Evt);
        }
    }
}