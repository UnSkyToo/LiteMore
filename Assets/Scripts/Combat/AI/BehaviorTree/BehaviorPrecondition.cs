namespace LiteMore.Combat.AI.BehaviorTree
{
    public abstract class BehaviorPrecondition
    {
        public abstract bool ExternalCondition(BehaviorInputData Input);
    }

    public class BehaviorPreconditionTrue : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return true;
        }
    }

    public class BehaviorPreconditionFalse : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return false;
        }
    }

    public class BehaviorPreconditionNot : BehaviorPrecondition
    {
        protected readonly BehaviorPrecondition Left_;

        public BehaviorPreconditionNot(BehaviorPrecondition Left)
        {
            Left_ = Left;
        }

        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return !Left_.ExternalCondition(Input);
        }
    }

    public class BehaviorPreconditionAnd : BehaviorPrecondition
    {
        protected readonly BehaviorPrecondition Left_;
        protected readonly BehaviorPrecondition Right_;

        public BehaviorPreconditionAnd(BehaviorPrecondition Left, BehaviorPrecondition Right)
        {
            Left_ = Left;
            Right_ = Right;
        }

        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return Left_.ExternalCondition(Input) && Right_.ExternalCondition(Input);
        }
    }

    public class BehaviorPreconditionOr : BehaviorPrecondition
    {
        protected readonly BehaviorPrecondition Left_;
        protected readonly BehaviorPrecondition Right_;

        public BehaviorPreconditionOr(BehaviorPrecondition Left, BehaviorPrecondition Right)
        {
            Left_ = Left;
            Right_ = Right;
        }

        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return Left_.ExternalCondition(Input) || Right_.ExternalCondition(Input);
        }
    }

    public class BehaviorPreconditionXor : BehaviorPrecondition
    {
        protected readonly BehaviorPrecondition Left_;
        protected readonly BehaviorPrecondition Right_;

        public BehaviorPreconditionXor(BehaviorPrecondition Left, BehaviorPrecondition Right)
        {
            Left_ = Left;
            Right_ = Right;
        }

        public override bool ExternalCondition(BehaviorInputData Input)
        {
            return Left_.ExternalCondition(Input) ^ Right_.ExternalCondition(Input);
        }
    }
}