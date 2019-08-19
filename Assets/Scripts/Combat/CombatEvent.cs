using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
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
        public uint SkillID { get; }
        public SkillArgs Args { get; }

        public NpcSkillEvent(uint MasterID, CombatTeam MasterTeam, uint SkillID, SkillArgs Args)
            : base(MasterID, MasterTeam)
        {
            this.SkillID = SkillID;
            this.Args = Args;
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

    public class NpcSkillChangedEvent : CombatEvent
    {
        public uint SkillID { get; }
        public bool IsAdd { get; }

        public NpcSkillChangedEvent(uint MasterID, CombatTeam MasterTeam, uint SkillID, bool IsAdd)
            : base(MasterID, MasterTeam)
        {
            this.SkillID = SkillID;
            this.IsAdd = IsAdd;
        }
    }
}