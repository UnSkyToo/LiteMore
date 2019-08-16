﻿using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.AI.Locking
{
    public class LockingRule
    {
        public LockTeamType TeamType { get; }
        public LockNpcType NpcType { get; }

        public LockingRule(LockTeamType TeamType, LockNpcType NpcType)
        {
            this.TeamType = TeamType;
            this.NpcType = NpcType;
        }

        public static readonly LockingRule Nearest = new LockingRule(LockTeamType.Enemy, LockNpcType.Nearest);
    }

    public static class LockingHelper
    {
        public static List<BaseNpc> Find(BaseNpc Master, LockingRule Rule, object Args)
        {
            var List = LockingTeam.Find(Master, Rule.TeamType, Args);
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