using System.Collections.Generic;
using LiteMore.Combat.Npc;

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
        public static List<BaseNpc> Find(BaseNpc Master, LockingRule Rule, object Args)
        {
            var List = LockingTeam.Find(Master, Rule.TeamType, Args);
            List = LockingRange.Find(List, Master, Rule.RangeType, Args);
            return LockingNpc.Find(List, Master, Rule.NpcType, Args);
        }

        public static BaseNpc FindNearest(BaseNpc Master)
        {
            var List = Find(Master, LockingRule.Nearest, null);

            if (List.Count > 0)
            {
                return List[0];
            }

            return null;
        }
    }
}