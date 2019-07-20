using UnityEngine;

namespace LiteMore.Motion
{
    public class MotionMove : MotionBase
    {
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Vector3 BeginPosition_;
        private Vector3 EndPosition_;
        private bool IsRelative_;

        public MotionMove(float Time, Vector3 Position, bool IsRelative)
            : base()
        {
            TotalTime_ = Time;
            BeginPosition_ = Position;
            EndPosition_ = Position;
            IsRelative_ = IsRelative;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BeginPosition_ = Master.localPosition;

            if (IsRelative_)
            {
                EndPosition_ += BeginPosition_;
            }
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ += DeltaTime;
            var T = CurrentTime_ / TotalTime_;
            if (T >= 1.0f)
            {
                Master.localPosition = EndPosition_;
                IsEnd = true;
            }

            Master.localPosition = Vector3.Lerp(BeginPosition_, EndPosition_, T);
        }
    }
}