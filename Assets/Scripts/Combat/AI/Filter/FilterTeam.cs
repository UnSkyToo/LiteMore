using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.AI.Filter
{
    public static class FilterTeam
    {
        private static readonly Dictionary<FilterTeamType, Func<BaseSkill, List<BaseNpc>>> FuncList_ = new Dictionary<FilterTeamType, Func<BaseSkill, List<BaseNpc>>>
        {
            {FilterTeamType.Self, Find_Self},
            {FilterTeamType.Team, Find_Team},
            {FilterTeamType.TeamExceptSelf, Find_TeamExceptSelf},
            {FilterTeamType.Enemy, Find_Enemy},
            {FilterTeamType.All, Find_All},
            {FilterTeamType.Attacker, Find_Attacker},
        };

        public static List<BaseNpc> Find(BaseSkill Skill, FilterTeamType Type)
        {
            var NpcList = FuncList_[Type].Invoke(Skill);
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

        private static List<BaseNpc> Find_Self(BaseSkill Skill)
        {
            return new List<BaseNpc>
            {
                Skill.Master
            };
        }

        private static List<BaseNpc> Find_Team(BaseSkill Skill)
        {
            return NpcManager.GetNpcList(Skill.Master.Team);
        }

        private static List<BaseNpc> Find_TeamExceptSelf(BaseSkill Skill)
        {
            var Result = Find_Team(Skill);
            Result.Remove(Skill.Master);
            return Result;
        }

        private static List<BaseNpc> Find_Enemy(BaseSkill Skill)
        {
            return NpcManager.GetNpcList(Skill.Master.Team.Opposite());
        }

        private static List<BaseNpc> Find_All(BaseSkill Skill)
        {
            var Result = Find_Team(Skill);
            Result.AddRange(Find_Enemy(Skill));
            return Result;
        }

        private static List<BaseNpc> Find_Attacker(BaseSkill Skill)
        {
            var Result = new List<BaseNpc>();
            if (Skill.Master.Action.IsValidAttacker())
            {
                Result.Add(Skill.Master);
            }
            return Result;
        }
    }
}