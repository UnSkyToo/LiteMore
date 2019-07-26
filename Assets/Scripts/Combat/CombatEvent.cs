using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat
{
    public abstract class CombatEvent : BaseEvent
    {
        public BaseNpc Master { get; }

        protected CombatEvent(BaseNpc Master)
        {
            this.Master = Master;
        }
    }

    public class NpcIdleEvent : CombatEvent
    {
        public NpcIdleEvent(BaseNpc Master)
            : base(Master)
        {
        }
    }

    public class NpcWalkEvent : CombatEvent
    {
        public Vector2 TargetPos { get; }

        public NpcWalkEvent(BaseNpc Master, Vector2 TargetPos)
            : base(Master)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class NpcAttackEvent : CombatEvent
    {
        public NpcAttackEvent(BaseNpc Master)
            : base(Master)
        {
        }
    }

    public class NpcDieEvent : CombatEvent
    {
        public NpcDieEvent(BaseNpc Master)
            : base(Master)
        {
        }
    }

    public class NpcBackEvent : CombatEvent
    {
        public Vector2 BackPos { get; }
        public float BackTime { get; }

        public NpcBackEvent(BaseNpc Master, Vector2 BackPos, float BackTime)
            : base(Master)
        {
            this.BackPos = BackPos;
            this.BackTime = BackTime;
        }
    }

    public class NpcHitTargetEvent : CombatEvent
    {
        public int Damage { get; }

        public NpcHitTargetEvent(BaseNpc Master, int Damage)
            : base(Master)
        {
            this.Damage = Damage;
        }
    }
}