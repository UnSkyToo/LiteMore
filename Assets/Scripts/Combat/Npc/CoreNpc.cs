using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class CoreNpc : BaseNpc
    {
        public CoreNpc(Transform Trans, float[] InitAttr)
            : base(Trans, CombatTeam.A, InitAttr)
        {
        }
    }
}