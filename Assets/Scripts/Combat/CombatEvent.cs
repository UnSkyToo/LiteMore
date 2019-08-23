using LiteFramework.Core.Event;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
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

    public class NpcAttrChangedEvent : CombatEvent
    {
        public NpcAttrIndex Index { get; }
        public float OldValue { get; }
        public float NewValue { get; }
        public float ChangeValue { get; }

        public NpcAttrChangedEvent(BaseNpc Master, NpcAttrIndex Index, float OldValue, float NewValue)
            : base(Master)
        {
            this.Index = Index;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
            this.ChangeValue = NewValue - OldValue;
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

    public class NpcSkillEvent : CombatEvent
    {
        public uint SkillID { get; }
        public SkillArgs Args { get; }

        public NpcSkillEvent(BaseNpc Master, uint SkillID, SkillArgs Args)
            : base(Master)
        {
            this.SkillID = SkillID;
            this.Args = Args;
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

    public class NpcDamageEvent : CombatEvent
    {
        public uint AttackerID { get; }
        public string SourceName { get; }
        public float Damage { get; }
        public float RealValue { get; }

        public NpcDamageEvent(BaseNpc Master, uint AttackerID, string SourceName, float Damage, float RealValue)
            : base(Master)
        {
            this.AttackerID = AttackerID;
            this.SourceName = SourceName;
            this.Damage = Damage;
            this.RealValue = RealValue;
        }
    }

    public class NpcSkillChangedEvent : CombatEvent
    {
        public uint SkillID { get; }
        public bool IsAdd { get; }

        public NpcSkillChangedEvent(BaseNpc Master, uint SkillID, bool IsAdd)
            : base(Master)
        {
            this.SkillID = SkillID;
            this.IsAdd = IsAdd;
        }
    }

    public class NpcAddEvent : CombatEvent
    {
        public NpcAddEvent(BaseNpc Master)
            : base(Master)
        {
        }
    }
}