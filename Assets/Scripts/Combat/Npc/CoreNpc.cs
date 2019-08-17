using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class CoreNpc : BaseNpc
    {
        public CoreNpc(string Name, Transform Trans, NpcAttribute InitAttr)
            : base(Name, Trans, CombatTeam.A, InitAttr)
        {
        }
    }
}