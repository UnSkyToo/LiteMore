using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.AI.Filter
{
    public static class FilterRange
    {
        private static readonly Dictionary<FilterRangeType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>> FuncList_ = new Dictionary<FilterRangeType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>>
        {
            {FilterRangeType.All, Find_All},
            {FilterRangeType.InDistance, Find_InDistance},
            {FilterRangeType.InShape, Find_InShape},
        };

        public static List<BaseNpc> Find(List<BaseNpc> List, BaseSkill Skill, FilterRangeType Type)
        {
            return FuncList_[Type].Invoke(List, Skill);
        }

        private static List<BaseNpc> Find_All(List<BaseNpc> List, BaseSkill Skill)
        {
            return List;
        }

        private static List<BaseNpc> Find_InDistance(List<BaseNpc> List, BaseSkill Skill)
        {
            var Result = new List<BaseNpc>();
            var Range = Skill.Radius;

            foreach (var Npc in List)
            {
                var Dist = Vector2.Distance(Skill.Master.Position, Npc.Position);
                if (Dist <= Range + CombatHelper.GetNpcHitRange(Npc))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }

        private static List<BaseNpc> Find_InShape(List<BaseNpc> List, BaseSkill Skill)
        {
            var Result = new List<BaseNpc>();
            var Shape = Skill.Shape;

            foreach (var Npc in List)
            {
                if (Shape.Contains(Skill.Master.Position, Npc.Position, Skill.Master.GetRotation()))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }
    }
}