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
        public bool Enabled { get; set; }
        public float DeltaTime { get; set; }
        public float TickTime { get; set; }
        public BaseNpc Attacker { get; set; }
        public float Distance { get; set; }
        public BehaviorNode Node { get; set; }
    }
}