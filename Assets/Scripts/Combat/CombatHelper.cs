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
    }
}