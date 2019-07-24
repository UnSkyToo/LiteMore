using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.AI.BehaviorTree
{
    public enum BehaviorRunningState : byte
    {
        Running,
        Finish,
        Error
    }

    public enum BehaviorParallelMode : byte
    {
        Or,
        And
    }

    public enum BehaviorTerminalState : byte
    {
        Ready,
        Running,
        Finish
    }

    public class BehaviorInputData
    {
        public float DeltaTime { get; set; }
        public BaseNpc Attacker { get; set; }
        public uint TargetID { get; set; }
        public uint FollowID { get; set; }

        public List<uint> Targets { get; set; }
    }
}