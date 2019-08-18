using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.AI.Locking
{
    public static class LockingRange
    {
        private static readonly Dictionary<LockRangeType, Func<List<BaseNpc>, BaseNpc, object, List<BaseNpc>>> FuncList_ = new Dictionary<LockRangeType, Func<List<BaseNpc>, BaseNpc, object, List<BaseNpc>>>
        {
            {LockRangeType.All, Find_All},
            {LockRangeType.InDistance, Find_InDistance},
            {LockRangeType.InShape, Find_InShape},
        };

        public static List<BaseNpc> Find(List<BaseNpc> List, BaseNpc Master, LockRangeType Type, object Args)
        {
            return FuncList_[Type].Invoke(List, Master, Args);
        }

        private static List<BaseNpc> Find_All(List<BaseNpc> List, BaseNpc Master, object Args)
        {
            return List;
        }

        private static List<BaseNpc> Find_InDistance(List<BaseNpc> List, BaseNpc Master, object Args)
        {
            var Result = new List<BaseNpc>();
            var Range = (float)Args;

            foreach (var Npc in List)
            {
                var Dist = Vector2.Distance(Master.Position, Npc.Position);
                if (Dist < Range + Npc.CalcFinalAttr(NpcAttrIndex.Radius))
                {
                    Result.Add(Npc);
                }
            }

            return Result;
        }

        private static List<BaseNpc> Find_InShape(List<BaseNpc> List, BaseNpc Master, object Args)
        {
            var Result = new List<BaseNpc>();
            var Shape = (BaseShape)Args;

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