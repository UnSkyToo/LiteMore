using LiteMore.Combat;
using LiteMore.Combat.Npc;

namespace Lite.Combat.Npc.Handler
{
    public class BaseNpcHandler
    {
        public uint Priority { get; }
        public BaseNpc Master { get; set; }

        public BaseNpcHandler(uint Priority)
        {
            this.Priority = Priority;
        }

        public virtual void OnTick(float DeltaTime)
        {
        }

        public virtual float OnAddAttr(NpcAttrIndex Index, float Value)
        {
            return Value;
        }
    }
}