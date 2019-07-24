namespace LiteMore.Combat.AI.BehaviorTree
{
    public class BehaviorNode
    {
        public const int InvalidIndex = -1;
        public const int MaxChildCount = 16;

        public string Name { get; set; }

        protected BehaviorNode Parent_;
        protected BehaviorNode[] Children_;
        protected int ChildCount_;
        protected BehaviorPrecondition Condition_;
        protected BehaviorNode ActiveNode_;
        protected BehaviorNode PreviousActiveNode_;

        public BehaviorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null)
        {
            this.Name = Name;

            Parent_ = Parent;
            Children_ = new BehaviorNode[MaxChildCount];
            ChildCount_ = 0;
            Condition_ = null;
            ActiveNode_ = null;
            PreviousActiveNode_ = null;

            SetCondition(Condition);
        }

        public BehaviorNode AddChild(BehaviorNode Child)
        {
            Children_[ChildCount_] = Child;
            ChildCount_++;
            return Child;
        }

        public BehaviorNode SetParent(BehaviorNode Parent)
        {
            Parent_ = Parent;
            return this;
        }

        public BehaviorPrecondition SetCondition(BehaviorPrecondition Condition)
        {
            Condition_ = Condition;
            return Condition_;
        }

        public void SetActiveNode(BehaviorNode Node)
        {
            PreviousActiveNode_ = ActiveNode_;
            ActiveNode_ = Node;
            Parent_?.SetActiveNode(Node);
        }

        public BehaviorNode GetPreviousActiveNode()
        {
            return ActiveNode_;
        }

        public bool Evaluate(BehaviorInputData Input)
        {
            return (Condition_ == null || Condition_.ExternalCondition(Input)) && OnEvaluate(Input);
        }

        public void Transition(BehaviorInputData Input)
        {
            OnTransition(Input);
        }

        public BehaviorRunningState Tick(BehaviorInputData Input)
        {
            return OnTick(Input);
        }

        protected virtual bool OnEvaluate(BehaviorInputData Input)
        {
            return true;
        }

        protected virtual void OnTransition(BehaviorInputData Input)
        {
        }

        protected virtual BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            return BehaviorRunningState.Finish;
        }

        protected bool IsValidIndex(int Index)
        {
            return Index >= 0 && Index < ChildCount_;
        }
    }

    public class BehaviorPrioritySelectorNode : BehaviorNode
    {
        protected int CurrentSelectIndex_;
        protected int PreviousSelectIndex_;

        public BehaviorPrioritySelectorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null)
            : base(Name, Parent, Condition)
        {
            CurrentSelectIndex_ = InvalidIndex;
            PreviousSelectIndex_ = InvalidIndex;
        }

        protected override bool OnEvaluate(BehaviorInputData Input)
        {
            CurrentSelectIndex_ = 0;

            for (var Index = 0; Index < ChildCount_; ++Index)
            {
                if (Children_[Index].Evaluate(Input))
                {
                    CurrentSelectIndex_ = Index;
                    return true;
                }
            }

            return false;
        }

        protected override void OnTransition(BehaviorInputData Input)
        {
            if (IsValidIndex(PreviousSelectIndex_))
            {
                Children_[PreviousSelectIndex_].Transition(Input);
            }

            PreviousSelectIndex_ = InvalidIndex;
        }

        protected override BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            var State = BehaviorRunningState.Finish;

            if (IsValidIndex(CurrentSelectIndex_))
            {
                if (PreviousSelectIndex_ != CurrentSelectIndex_)
                {
                    if (IsValidIndex(PreviousSelectIndex_))
                    {
                        Children_[PreviousSelectIndex_].Transition(Input);
                    }

                    PreviousSelectIndex_ = CurrentSelectIndex_;
                }
            }

            if (IsValidIndex(PreviousSelectIndex_))
            {
                State = Children_[PreviousSelectIndex_].Tick(Input);

                if (State == BehaviorRunningState.Finish)
                {
                    PreviousSelectIndex_ = InvalidIndex;
                }
            }

            return State;
        }
    }

    public class BehaviorNonePrioritySelectorNode : BehaviorPrioritySelectorNode
    {
        public BehaviorNonePrioritySelectorNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null)
            : base(Name, Parent, Condition)
        {
        }

        protected override bool OnEvaluate(BehaviorInputData Input)
        {
            if (IsValidIndex(CurrentSelectIndex_))
            {
                if (Children_[CurrentSelectIndex_].Evaluate(Input))
                {
                    return true;
                }
            }

            return base.OnEvaluate(Input);
        }
    }

    public class BehaviorSequenceNode : BehaviorNode
    {
        protected int CurrentSelectIndex_;

        public BehaviorSequenceNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null)
            : base(Name, Parent, Condition)
        {
            CurrentSelectIndex_ = InvalidIndex;
        }

        protected override bool OnEvaluate(BehaviorInputData Input)
        {
            var Index = 0;
            if (CurrentSelectIndex_ != InvalidIndex)
            {
                Index = CurrentSelectIndex_;
            }

            if (IsValidIndex(Index))
            {
                if (Children_[Index].Evaluate(Input))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnTransition(BehaviorInputData Input)
        {
            if (IsValidIndex(CurrentSelectIndex_))
            {
                Children_[CurrentSelectIndex_].Transition(Input);
            }

            CurrentSelectIndex_ = InvalidIndex;
        }

        protected override BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            var State = BehaviorRunningState.Finish;

            if (CurrentSelectIndex_ == InvalidIndex)
            {
                CurrentSelectIndex_ = 0;
            }

            State = Children_[CurrentSelectIndex_].Tick(Input);

            if (State == BehaviorRunningState.Finish)
            {
                CurrentSelectIndex_++;

                if (CurrentSelectIndex_ == ChildCount_)
                {
                    CurrentSelectIndex_ = InvalidIndex;
                }
                else
                {
                    State = BehaviorRunningState.Running;
                }
            }

            if (State == BehaviorRunningState.Error)
            {
                CurrentSelectIndex_ = InvalidIndex;
            }

            return State;
        }
    }

    public class BehaviorParallelNode : BehaviorNode
    {
        protected BehaviorParallelMode Mode_;
        protected BehaviorRunningState[] ChildrenState_;

        public BehaviorParallelNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null)
            : base(Name, Parent, Condition)
        {
            Mode_ = BehaviorParallelMode.Or;

            ChildrenState_ = new BehaviorRunningState[MaxChildCount];
            for (var Index = 0; Index < MaxChildCount; ++Index)
            {
                ChildrenState_[Index] = BehaviorRunningState.Running;
            }
        }

        public BehaviorParallelNode SetMode(BehaviorParallelMode Mode)
        {
            Mode_ = Mode;
            return this;
        }

        protected override bool OnEvaluate(BehaviorInputData Input)
        {
            for (var Index = 0; Index < ChildCount_; ++Index)
            {
                if (ChildrenState_[Index] == BehaviorRunningState.Running)
                {
                    if (!Children_[Index].Evaluate(Input))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void OnTransition(BehaviorInputData Input)
        {
            for (var Index = 0; Index < MaxChildCount; ++Index)
            {
                ChildrenState_[Index] = BehaviorRunningState.Running;
            }

            for (var Index = 0; Index < ChildCount_; ++Index)
            {
                Children_[Index].Transition(Input);
            }
        }

        protected override BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            var FinishedCount = 0;

            for (var Index = 0; Index < ChildCount_; ++Index)
            {
                if (Mode_ == BehaviorParallelMode.Or)
                {
                    if (ChildrenState_[Index] == BehaviorRunningState.Running)
                    {
                        ChildrenState_[Index] = Children_[Index].Tick(Input);
                    }

                    if (ChildrenState_[Index] != BehaviorRunningState.Running)
                    {
                        FinishedCount = ChildCount_;
                        break;
                    }
                }
                else if (Mode_ == BehaviorParallelMode.And)
                {
                    if (ChildrenState_[Index] == BehaviorRunningState.Running)
                    {
                        ChildrenState_[Index] = Children_[Index].Tick(Input);
                    }

                    if (ChildrenState_[Index] != BehaviorRunningState.Running)
                    {
                        FinishedCount++;
                    }
                }
            }

            if (FinishedCount == ChildCount_)
            {
                for (var Index = 0; Index < MaxChildCount; ++Index)
                {
                    ChildrenState_[Index] = BehaviorRunningState.Running;
                }

                return BehaviorRunningState.Finish;
            }

            return BehaviorRunningState.Running;
        }
    }

    public class BehaviorLoopNode : BehaviorNode
    {
        protected int CurrentCount_;
        protected int LoopCount_;

        public BehaviorLoopNode(string Name, BehaviorNode Parent, BehaviorPrecondition Condition = null, int LoopCount = -1)
            : base(Name, Parent, Condition)
        {
            LoopCount_ = LoopCount;
            CurrentCount_ = 0;
        }

        protected override bool OnEvaluate(BehaviorInputData Input)
        {
            var IsLoop = (LoopCount_ == -1) || CurrentCount_ < LoopCount_;

            if (!IsLoop)
            {
                return false;
            }

            if (IsValidIndex(0))
            {
                if (Children_[0].Evaluate(Input))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnTransition(BehaviorInputData Input)
        {
            if (IsValidIndex(0))
            {
                Children_[0].Transition(Input);
            }

            CurrentCount_ = 0;
        }

        protected override BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            var State = BehaviorRunningState.Finish;

            if (IsValidIndex(0))
            {
                State = Children_[0].Tick(Input);

                if (State == BehaviorRunningState.Finish)
                {
                    if (LoopCount_ != -1)
                    {
                        CurrentCount_++;
                        if (CurrentCount_ == LoopCount_)
                        {
                            State = BehaviorRunningState.Finish;
                        }
                    }
                    else
                    {
                        State = BehaviorRunningState.Running;
                    }
                }
            }

            if (State == BehaviorRunningState.Finish)
            {
                CurrentCount_ = 0;
            }

            return State;
        }
    }

    public class BehaviorTerminalNode : BehaviorNode
    {
        protected BehaviorTerminalState TerminalState_;
        protected bool NeedExit_;

        public BehaviorTerminalNode()
            : base(string.Empty, null, null)
        {
            TerminalState_ = BehaviorTerminalState.Ready;
            NeedExit_ = false;
        }

        protected override void OnTransition(BehaviorInputData Input)
        {
            if (NeedExit_)
            {
                OnExit(Input, BehaviorRunningState.Error);
            }

            SetActiveNode(null);
            TerminalState_ = BehaviorTerminalState.Ready;
            NeedExit_ = false;
        }

        protected override BehaviorRunningState OnTick(BehaviorInputData Input)
        {
            var State = BehaviorRunningState.Finish;

            if (TerminalState_ == BehaviorTerminalState.Ready)
            {
                OnEnter(Input);
                NeedExit_ = true;
                TerminalState_ = BehaviorTerminalState.Running;
                SetActiveNode(this);
            }

            if (TerminalState_ == BehaviorTerminalState.Running)
            {
                State = OnExecute(Input);
                SetActiveNode(this);

                if (State == BehaviorRunningState.Finish || State == BehaviorRunningState.Error)
                {
                    TerminalState_ = BehaviorTerminalState.Finish;
                }
            }

            if (TerminalState_ == BehaviorTerminalState.Finish)
            {
                if (NeedExit_)
                {
                    OnExit(Input, State);
                }

                TerminalState_ = BehaviorTerminalState.Ready;
                NeedExit_ = false;
                SetActiveNode(null);
            }

            return State;
        }

        protected virtual void OnEnter(BehaviorInputData Input)
        {
        }

        protected virtual BehaviorRunningState OnExecute(BehaviorInputData Input)
        {
            return BehaviorRunningState.Finish;
        }

        protected virtual void OnExit(BehaviorInputData Input, BehaviorRunningState ExitState)
        {
        }
    }
}