using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.AI.Locking
{
    public static class LockingRange
    {
        private static readonly Dictionary<LockRangeType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>> FuncList_ = new Dictionary<LockRangeType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>>
        {
            {LockRangeType.All, Find_All},
            {LockRangeType.InDistance, Find_InDistance},
            {LockRangeType.InShape, Find_InShape},
        };

        public static List<BaseNpc> Find(List<BaseNpc> List, BaseSkill Skill, LockRangeType Type)
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
                if (Dist < Range + Npc.CalcFinalAttr(NpcAttrIndex.Radius))
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
                if (Shape.Contains(Npc.Position))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }
    }
}