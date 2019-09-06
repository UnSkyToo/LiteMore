using LiteFramework.Interface;

namespace LiteMore.Combat.Npc.Handler
{
    public class BaseNpcHandler : IPriority
    {
        public int Priority { get; }
        public BaseNpc Master { get; set; }

        public BaseNpcHandler(int Priority)
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