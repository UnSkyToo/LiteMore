namespace LiteFramework.Core.Motion
{
    public class ParallelMotion : BaseMotion
    {
        private readonly BaseMotion[] SubMotions_;
        private readonly int Count_;

        public ParallelMotion(params BaseMotion[] Args)
            : base()
        {
            SubMotions_ = Args;
            Count_ = Args?.Length ?? 0;
            IsEnd = Count_ == 0;
        }

        public override void Enter()
        {
            for (var Index = 0; Index < Count_; ++Index)
            {
                SubMotions_[Index].Master = Master;
                SubMotions_[Index].Enter();
            }
        }

        public override void Exit()
        {
            for (var Index = 0; Index < Count_; ++Index)
            {
                SubMotions_[Index].Exit();
            }
        }

        public override void Tick(float DeltaTime)
        {
            var EndCount = 0;

            for (var Index = 0; Index < Count_; ++Index)
            {
                if (SubMotions_[Index].IsEnd)
                {
                    EndCount++;
                    continue;
                }

                SubMotions_[Index].Tick(DeltaTime);
            }

            if (EndCount == Count_)
            {
                IsEnd = true;
            }
        }
    }
}