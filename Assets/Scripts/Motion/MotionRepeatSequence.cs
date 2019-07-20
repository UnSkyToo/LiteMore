namespace LiteMore.Motion
{
    public class MotionRepeatSequence : MotionSequence
    {
        public MotionRepeatSequence(params MotionBase[] Args)
            : base(Args)
        {
        }

        protected override int GetNextMotionIndex()
        {
            Index_++;
            if (Index_ >= Count_)
            {
                return 0;
            }

            return Index_;
        }
    }
}