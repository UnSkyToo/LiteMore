namespace LiteMore.Motion
{
    public class MotionSequence : MotionBase
    {
        private readonly MotionBase[] SubMotions_;
        private MotionBase Current_;
        protected readonly int Count_;
        protected int Index_;

        public MotionSequence(params MotionBase[] Args)
            : base()
        {
            SubMotions_ = Args;
            Index_ = -1;
            Count_ = Args?.Length ?? 0;
            IsEnd = Count_ == 0;
        }

        public override void Enter()
        {
            ActiveNextMotion();
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
            if (Current_ != null)
            {
                if (Current_.IsEnd)
                {
                    ActiveNextMotion();
                }
                else
                {
                    Current_.Tick(DeltaTime);
                }
            }
        }

        protected virtual int GetNextMotionIndex()
        {
            Index_++;
            if (Index_ >= Count_)
            {
                return -1;
            }

            return Index_;
        }

        private void ActiveNextMotion()
        {
            Current_?.Exit();

            Index_ = GetNextMotionIndex();
            if (Index_ == -1)
            {
                Current_ = null;
                IsEnd = true;
            }
            else
            {
                Current_ = SubMotions_[Index_];
            }

            if (Current_ != null)
            {
                Current_.Master = Master;
                Current_?.Enter();
            }
        }
    }
}