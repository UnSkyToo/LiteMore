using LiteFramework.Core.Event;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat
{
    public abstract class CombatEvent : BaseEvent
    {
        public uint MasterID { get; }
        public CombatTeam MasterTeam { get; }

        protected CombatEvent(uint MasterID, CombatTeam MasterTeam)
        {
            this.MasterID = MasterID;
            this.MasterTeam = MasterTeam;
        }
    }

    public class NpcIdleEvent : CombatEvent
    {
        public NpcIdleEvent(uint MasterID, CombatTeam MasterTeam)
            : base(MasterID, MasterTeam)
        {
        }
    }

    public class NpcWalkEvent : CombatEvent
    {
        public Vector2 TargetPos { get; }

        public NpcWalkEvent(uint MasterID, CombatTeam MasterTeam, Vector2 TargetPos)
            : base(MasterID, MasterTeam)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class NpcSkillEvent : CombatEvent
    {
        public uint TargetID { get; }
        public uint SkillID { get; }

        public NpcSkillEvent(uint MasterID, CombatTeam MasterTeam, uint TargetID, uint SkillID)
            : base(MasterID, MasterTeam)
        {
            this.TargetID = TargetID;
            this.SkillID = SkillID;
        }
    }

    public class NpcDieEvent : CombatEvent
    {
        public NpcDieEvent(uint MasterID, CombatTeam MasterTeam)
            : base(MasterID, MasterTeam)
        {
        }
    }

    public class NpcBackEvent : CombatEvent
    {
        public Vector2 BackPos { get; }
        public float BackTime { get; }

        public NpcBackEvent(uint MasterID, CombatTeam MasterTeam, Vector2 BackPos, float BackTime)
            : base(MasterID, MasterTeam)
        {
            this.BackPos = BackPos;
            this.BackTime = BackTime;
        }
    }

    public class NpcHitTargetEvent : CombatEvent
    {
        public uint SkillID { get; }
        public string HitSfx { get; }

        public NpcHitTargetEvent(uint MasterID, CombatTeam MasterTeam, uint SkillID, string HitSfx)
            : base(MasterID, MasterTeam)
        {
            this.SkillID = SkillID;
            this.HitSfx = HitSfx;
        }
    }

    public class NpcDamageEvent : CombatEvent
    {
        public uint AttackerID { get; }
        public string SourceName { get; }
        public float Damage { get; }
        public float RealValue { get; }

        public NpcDamageEvent(uint MasterID, CombatTeam MasterTeam, uint AttackerID, string SourceName, float Damage, float RealValue)
            : base(MasterID, MasterTeam)
        {
            this.AttackerID = AttackerID;
            this.SourceName = SourceName;
            this.Damage = Damage;
            this.RealValue = RealValue;
        }
    }
}