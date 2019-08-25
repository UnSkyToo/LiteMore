using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat
{
    public static class CombatHelper
    {
        public static CombatTeam Opposite(this CombatTeam Team)
        {
            switch (Team)
            {
                case CombatTeam.A:
                    return CombatTeam.B;
                case CombatTeam.B:
                    return CombatTeam.A;
                default:
                    return Team;
            }
        }

        public static NpcDirection CalcDirection(Vector2 OriginPos, Vector2 TargetPos)
        {
            if (OriginPos.x < TargetPos.x)
            {
                return NpcDirection.Right;
            }

            return NpcDirection.Left;
        }

        /// <summary>
        /// 计算攻击范围
        /// </summary>
        public static float GetNpcAtkRange(BaseNpc Master)
        {
            var AtkRange = Master.CalcFinalAttr(NpcAttrIndex.AtkRange);

            if (AtkRange < 50)
            {
                return AtkRange * Master.Scale.x;
            }

            return AtkRange;
        }

        /// <summary>
        /// 计算受击范围
        /// </summary>
        public static float GetNpcHitRange(BaseNpc Master)
        {
            return Master.CalcFinalAttr(NpcAttrIndex.HitRange) * Master.Scale.x;
        }

        public static bool InAttackRange(BaseNpc Attacker, BaseNpc Target)
        {
            var Dist = Vector2.Distance(Attacker.Position, Target.Position);
            var Range = GetNpcAtkRange(Attacker) + GetNpcHitRange(Target);
            return Dist <= Range;
        }
    }
}