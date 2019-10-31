using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class RotateMotion : BaseMotion
    {
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Quaternion BeginRotate_;
        private Quaternion EndRotate_;

        public RotateMotion(float Time, Quaternion Rotate)
            : base()
        {
            TotalTime_ = Time;
            BeginRotate_ = Rotate;
            EndRotate_ = Rotate;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BeginRotate_ = Master.localRotation;
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
                Master.localRotation = EndRotate_;
                IsEnd = true;
            }

            Master.localRotation = Quaternion.Lerp(BeginRotate_, EndRotate_, T);
        }
    }
}