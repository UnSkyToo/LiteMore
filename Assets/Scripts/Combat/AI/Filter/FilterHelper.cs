using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
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

    public class FilterArgs
    {
        public BaseNpc Master { get; set; }
        public CombatTeam Team { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public BaseShape Shape { get; set; }
        public Quaternion Rotation { get; set; }
    }

    public static class FilterHelper
    {
        public static List<BaseNpc> Find(FilterRule Rule, FilterArgs Args)
        {
            var List = FilterTeam.Find(Rule.TeamType, Args);
            List = FilterRange.Find(List, Rule.RangeType, Args);
            return FilterNpc.Find(List, Rule.NpcType, Args);
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