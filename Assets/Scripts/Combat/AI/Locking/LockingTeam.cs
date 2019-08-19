using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.AI.Locking
{
    public static class LockingTeam
    {
        private static readonly Dictionary<LockTeamType, Func<BaseSkill, List<BaseNpc>>> FuncList_ = new Dictionary<LockTeamType, Func<BaseSkill, List<BaseNpc>>>
        {
            {LockTeamType.Self, Find_Self},
            {LockTeamType.Team, Find_Team},
            {LockTeamType.TeamExceptSelf, Find_TeamExceptSelf},
            {LockTeamType.Enemy, Find_Enemy},
            {LockTeamType.All, Find_All},
            {LockTeamType.Attacker, Find_Attacker},
        };

        public static List<BaseNpc> Find(BaseSkill Skill, LockTeamType Type)
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
            if (Skill.Master.IsValidAttacker())
            {
                Result.Add(Skill.Master);
            }
            return Result;
        }
    }
}