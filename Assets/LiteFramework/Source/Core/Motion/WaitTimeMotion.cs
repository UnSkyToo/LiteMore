namespace LiteFramework.Core.Motion
{
    public class WaitTimeMotion : BaseMotion
    {
        private readonly float TotalTime_;
        private float CurrentTime_;

        public WaitTimeMotion(float Time)
            : base()
        {
            TotalTime_ = Time;
        }

        public override void Enter()
        {
            CurrentTime_ = TotalTime_;
            IsEnd = false;
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ -= DeltaTime;

            if (CurrentTime_ <= 0.0f)
            {
                IsEnd = true;
            }
        }
    }
}