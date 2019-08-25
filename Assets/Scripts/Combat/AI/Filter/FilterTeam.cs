using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.AI.Filter
{
    public static class FilterTeam
    {
        private static readonly Dictionary<FilterTeamType, Func<FilterArgs, List<BaseNpc>>> FuncList_ = new Dictionary<FilterTeamType, Func<FilterArgs, List<BaseNpc>>>
        {
            {FilterTeamType.Self, Find_Self},
            {FilterTeamType.Team, Find_Team},
            {FilterTeamType.TeamExceptSelf, Find_TeamExceptSelf},
            {FilterTeamType.Enemy, Find_Enemy},
            {FilterTeamType.All, Find_All},
            {FilterTeamType.Attacker, Find_Attacker},
        };

        public static List<BaseNpc> Find(FilterTeamType Type, FilterArgs Args)
        {
            var NpcList = FuncList_[Type].Invoke(Args);
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

        private static List<BaseNpc> Find_Self(FilterArgs Args)
        {
            return new List<BaseNpc>
            {
                Args.Master
            };
        }

        private static List<BaseNpc> Find_Team(FilterArgs Args)
        {
            return NpcManager.GetNpcList(Args.Team);
        }

        private static List<BaseNpc> Find_TeamExceptSelf(FilterArgs Args)
        {
            var Result = Find_Team(Args);
            Result.Remove(Args.Master);
            return Result;
        }

        private static List<BaseNpc> Find_Enemy(FilterArgs Args)
        {
            return NpcManager.GetNpcList(Args.Team.Opposite());
        }

        private static List<BaseNpc> Find_All(FilterArgs Args)
        {
            var Result = Find_Team(Args);
            Result.AddRange(Find_Enemy(Args));
            return Result;
        }

        private static List<BaseNpc> Find_Attacker(FilterArgs Args)
        {
            var Result = new List<BaseNpc>();
            if (Args.Master.Action.IsValidAttacker())
            {
                Result.Add(Args.Master);
            }
            return Result;
        }
    }
}