using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public abstract class NpcEvent : BaseEvent
    {
        public uint ID { get; }

        protected NpcEvent(uint ID)
        {
            this.ID = ID;
        }
    }

    public class NpcIdleEvent : NpcEvent
    {
        public NpcIdleEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcMoveEvent : NpcEvent
    {
        public Vector2 TargetPos { get; }

        public NpcMoveEvent(uint ID, Vector2 TargetPos)
            : base(ID)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class NpcAttackEvent : NpcEvent
    {
        public NpcAttackEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcDieEvent : NpcEvent
    {
        public NpcDieEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcBackEvent : NpcEvent
    {
        public Vector2 BackPos { get; }
        public float BackTime { get; }

        public NpcBackEvent(uint ID, Vector2 BackPos, float BackTime)
            : base(ID)
        {
            this.BackPos = BackPos;
            this.BackTime = BackTime;
        }
    }
}