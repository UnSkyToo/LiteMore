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

        public static bool IsAttackRange(BaseNpc Attacker, BaseNpc Target)
        {
            var Dist = Vector2.Distance(Attacker.Position, Target.Position);
            var Range = Attacker.CalcFinalAttr(NpcAttrIndex.Range) + Target.CalcFinalAttr(NpcAttrIndex.Radius);
            return Dist <= Range;
        }
    }
}