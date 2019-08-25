using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.AI.Filter
{
    public static class FilterRange
    {
        private static readonly Dictionary<FilterRangeType, Func<List<BaseNpc>, FilterArgs, List<BaseNpc>>> FuncList_ = new Dictionary<FilterRangeType, Func<List<BaseNpc>, FilterArgs, List<BaseNpc>>>
        {
            {FilterRangeType.All, Find_All},
            {FilterRangeType.InDistance, Find_InDistance},
            {FilterRangeType.InShape, Find_InShape},
        };

        public static List<BaseNpc> Find(List<BaseNpc> List, FilterRangeType Type, FilterArgs Args)
        {
            return FuncList_[Type].Invoke(List, Args);
        }

        private static List<BaseNpc> Find_All(List<BaseNpc> List, FilterArgs Args)
        {
            return List;
        }

        private static List<BaseNpc> Find_InDistance(List<BaseNpc> List, FilterArgs Args)
        {
            var Result = new List<BaseNpc>();
            var Range = Args.Radius;

            foreach (var Npc in List)
            {
                var Dist = Vector2.Distance(Args.Position, Npc.Position);
                if (Dist <= Range + CombatHelper.GetNpcHitRange(Npc))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }

        private static List<BaseNpc> Find_InShape(List<BaseNpc> List, FilterArgs Args)
        {
            var Result = new List<BaseNpc>();
            var Shape = Args.Shape;

            foreach (var Npc in List)
            {
                if (Shape.Contains(Args.Position, Npc.Position, Args.Rotation))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }
    }
}