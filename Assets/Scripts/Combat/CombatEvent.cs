using UnityEngine;

namespace LiteMore.Combat
{
    public abstract class CombatEvent : BaseEvent
    {
        public uint ID { get; }

        protected CombatEvent(uint ID)
        {
            this.ID = ID;
        }
    }

    public class NpcIdleEvent : CombatEvent
    {
        public NpcIdleEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcWalkEvent : CombatEvent
    {
        public Vector2 TargetPos { get; }

        public NpcWalkEvent(uint ID, Vector2 TargetPos)
            : base(ID)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class NpcAttackEvent : CombatEvent
    {
        public NpcAttackEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcDieEvent : CombatEvent
    {
        public NpcDieEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class NpcBackEvent : CombatEvent
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

    public class NpcHitTargetEvent : CombatEvent
    {
        public int Damage { get; }

        public NpcHitTargetEvent(uint ID, int Damage)
            : base(ID)
        {
            this.Damage = Damage;
        }
    }
}