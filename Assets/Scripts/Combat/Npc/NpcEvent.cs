using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public abstract class NpcEvent : EventBase
    {
        public uint ID { get; }

        protected NpcEvent(uint ID)
        {
            this.ID = ID;
        }
    }

    public class IdleEvent : NpcEvent
    {
        public IdleEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class MoveEvent : NpcEvent
    {
        public Vector2 TargetPos { get; }

        public MoveEvent(uint ID, Vector2 TargetPos)
            : base(ID)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class AttackEvent : NpcEvent
    {
        public AttackEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class DieEvent : NpcEvent
    {
        public DieEvent(uint ID)
            : base(ID)
        {
        }
    }
}