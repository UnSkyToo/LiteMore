using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Skill
{
    public class SkillArgs
    {
        public BaseSkill Skill { get; }
        public BaseNpc Master { get; }
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public FilterRule LockRule { get; set; }
        public RectTransform CancelObj { get; set; }
        public string HitSfx { get; set; }

        public SkillArgs(BaseSkill Skill)
        {
            this.Skill = Skill;
            this.Master = Skill.Master;
        }

        public bool CanUse()
        {
            if (Skill == null || Skill.Master == null)
            {
                return false;
            }

            return Skill.Master.CalcFinalAttr(NpcAttrIndex.Mp) >= Skill.Cost;
        }
    }
}