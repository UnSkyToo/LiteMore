using UnityEngine;

namespace LiteMore.Motion
{
    public class MotionScale : MotionBase
    {
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Vector3 BeginScale_;
        private Vector3 EndScale_;
        private bool IsRelative_;

        public MotionScale(float Time, Vector3 Scale, bool IsRelative)
            : base()
        {
            TotalTime_ = Time;
            BeginScale_ = Scale;
            EndScale_ = Scale;
            IsRelative_ = IsRelative;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BeginScale_ = Master.localScale;

            if (IsRelative_)
            {
                EndScale_ += BeginScale_;
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
                Master.localScale = EndScale_;
                IsEnd = true;
            }

            Master.localScale = Vector3.Lerp(BeginScale_, EndScale_, T);
        }
    }
}