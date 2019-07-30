using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class CoreNpc : BaseNpc
    {
        public CoreNpc(string Name, Transform Trans, float[] InitAttr)
            : base(Name, Trans, CombatTeam.A, InitAttr)
        {
        }
    }
}