namespace LiteFramework.Core.Motion
{
    public class RepeatSequenceMotion : SequenceMotion
    {
        public RepeatSequenceMotion(params BaseMotion[] Args)
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