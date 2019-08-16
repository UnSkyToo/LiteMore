using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.AI.Locking
{
    public static class LockingTeam
    {
        private static readonly Dictionary<LockTeamType, Func<BaseNpc, List<BaseNpc>>> FuncList_ = new Dictionary<LockTeamType, Func<BaseNpc, List<BaseNpc>>>
        {
            {LockTeamType.Self, Find_Self},
            {LockTeamType.Team, Find_Team},
            {LockTeamType.TeamExceptSelf, Find_TeamExceptSelf},
            {LockTeamType.Enemy, Find_Enemy},
            {LockTeamType.All, Find_All},
            {LockTeamType.Attacker, Find_Attacker},
        };

        public static List<BaseNpc> Find(BaseNpc Master, LockTeamType Type)
        {
            var NpcList = FuncList_[Type].Invoke(Master);
            var Result = new List<BaseNpc>();

            foreach (var Npc in NpcList)
            {
                if (Npc.IsValid())
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }

        private static List<BaseNpc> Find_Self(BaseNpc Master)
        {
            return new List<BaseNpc>
            {
                Master
            };
        }

        private static List<BaseNpc> Find_Team(BaseNpc Master)
        {
            return NpcManager.GetNpcList(Master.Team);
        }

        private static List<BaseNpc> Find_TeamExceptSelf(BaseNpc Master)
        {
            var Result = Find_Team(Master);
            Result.Remove(Master);
            return Result;
        }

        private static List<BaseNpc> Find_Enemy(BaseNpc Master)
        {
            return NpcManager.GetNpcList(Master.Team.Opposite());
        }

        private static List<BaseNpc> Find_All(BaseNpc Master)
        {
            var Result = Find_Team(Master);
            Result.AddRange(Find_Enemy(Master));
            return Result;
        }

        private static List<BaseNpc> Find_Attacker(BaseNpc Master)
        {
            var Result = new List<BaseNpc>();
            if (Master.IsValidAttacker())
            {
                Result.Add(Master);
            }
            return Result;
        }
    }
}