using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.AI.Filter
{
    public class FilterRule
    {
        public FilterTeamType TeamType { get; }
        public FilterRangeType RangeType { get; }
        public FilterNpcType NpcType { get; }

        public FilterRule(FilterTeamType TeamType, FilterRangeType RangeType, FilterNpcType NpcType)
        {
            this.TeamType = TeamType;
            this.RangeType = RangeType;
            this.NpcType = NpcType;
        }

        public static readonly FilterRule All = new FilterRule(FilterTeamType.All, FilterRangeType.All, FilterNpcType.All);
        public static readonly FilterRule Self = new FilterRule(FilterTeamType.Self, FilterRangeType.All, FilterNpcType.All);
        public static readonly FilterRule Nearest = new FilterRule(FilterTeamType.Enemy, FilterRangeType.All, FilterNpcType.Nearest);
    }

    public static class FilterHelper
    {
        public static List<BaseNpc> Find(BaseSkill Skill, FilterRule Rule)
        {
            var List = FilterTeam.Find(Skill, Rule.TeamType);
            List = FilterRange.Find(List, Skill, Rule.RangeType);
            return FilterNpc.Find(List, Skill, Rule.NpcType);
        }

        public static BaseNpc FindNearest(BaseNpc Master)
        {
            var List = NpcManager.GetNpcList(Master.Team.Opposite());

            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Dist = Vector2.Distance(Master.Position, Npc.Position);
                if (Dist < Value)
                {
                    Value = Dist;
                    Target = Npc;
                }
            }

            return Target;
        }
    }
}