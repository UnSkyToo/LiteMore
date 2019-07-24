namespace LiteMore.Combat.AI.BehaviorTree
{
    public static class BehaviorFactory
    {
        public static BehaviorNode CreateBehaviorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition)
        {
            var Node = new BehaviorNode(Name, Parent, Condition);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreatePrioritySelectorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition)
        {
            var Node = new BehaviorPrioritySelectorNode(Name, Parent, Condition);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreateNonePrioritySelectorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition)
        {
            var Node = new BehaviorNonePrioritySelectorNode(Name, Parent, Condition);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreateSequenceNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition)
        {
            var Node = new BehaviorSequenceNode(Name, Parent, Condition);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreateParallelNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition, BehaviorParallelMode Mode)
        {
            var Node = new BehaviorParallelNode(Name, Parent, Condition);
            Node.SetMode(Mode);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreateLoopNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition)
        {
            var Node = new BehaviorLoopNode(Name, Parent, Condition);
            Parent?.AddChild(Node);
            return Node;
        }

        public static BehaviorNode CreateTerminalNode<T>(string Name, BehaviorNode Parent, BehaviorPrecondition Condition) where T : BehaviorTerminalNode, new()
        {
            //var Node = new T(Name, Parent, Condition);
            var Node = default(T);
            Node.Name = Name;
            Node.SetParent(Parent);
            Node.SetCondition(Condition);
            Parent?.AddChild(Node);
            return Node;
        }
    }
}