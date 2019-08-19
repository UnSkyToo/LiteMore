using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.AI.Locking
{
    public class LockingRule
    {
        public LockTeamType TeamType { get; }
        public LockRangeType RangeType { get; }
        public LockNpcType NpcType { get; }

        public LockingRule(LockTeamType TeamType, LockRangeType RangeType, LockNpcType NpcType)
        {
            this.TeamType = TeamType;
            this.RangeType = RangeType;
            this.NpcType = NpcType;
        }

        public static readonly LockingRule All = new LockingRule(LockTeamType.All, LockRangeType.All, LockNpcType.All);
        public static readonly LockingRule Self = new LockingRule(LockTeamType.Self, LockRangeType.All, LockNpcType.All);
        public static readonly LockingRule Nearest = new LockingRule(LockTeamType.Enemy, LockRangeType.All, LockNpcType.Nearest);
    }

    public static class LockingHelper
    {
        public static List<BaseNpc> Find(BaseSkill Skill, LockingRule Rule)
        {
            var List = LockingTeam.Find(Skill, Rule.TeamType);
            List = LockingRange.Find(List, Skill, Rule.RangeType);
            return LockingNpc.Find(List, Skill, Rule.NpcType);
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