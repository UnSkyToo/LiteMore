using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class MoveMotion : BaseMotion
    {
        private readonly bool IsRelative_;
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Vector3 BeginPosition_;
        private Vector3 EndPosition_;

        public MoveMotion(float Time, Vector3 Position, bool IsRelative)
            : base()
        {
            IsRelative_ = IsRelative;
            TotalTime_ = Time;
            BeginPosition_ = Position;
            EndPosition_ = Position;
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