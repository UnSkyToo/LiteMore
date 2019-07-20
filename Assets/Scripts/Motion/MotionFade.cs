using UnityEngine;

namespace LiteMore.Motion
{
    public class MotionFade : MotionBase
    {
        private readonly float TotalTime_;
        private float CurrentTime_;
        private float BeginAlpha_;
        private float EndAlpha_;
        private bool IsRelative_;

        private CanvasGroup Group_;

        public MotionFade(float Time, float Alpha, bool IsRelative)
            : base()
        {
            TotalTime_ = Time;
            BeginAlpha_ = Alpha;
            EndAlpha_ = Alpha;
            IsRelative_ = IsRelative;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            Group_ = Master.GetComponent<CanvasGroup>();
            if (Group_ == null)
            {
                Group_ = Master.gameObject.AddComponent<CanvasGroup>();
            }

            BeginAlpha_ = Group_.alpha;

            if (IsRelative_)
            {
                EndAlpha_ += BeginAlpha_;
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
                Group_.alpha = EndAlpha_;
                IsEnd = true;
            }

            Group_.alpha = Mathf.Lerp(BeginAlpha_, EndAlpha_, T);
        }
    }

    public class MotionFadeIn : MotionFade
    {
        public MotionFadeIn(float Time)
            : base(Time, 1.0f, false)
        {
        }
    }

    public class MotionFadeOut : MotionFade
    {
        public MotionFadeOut(float Time)
            : base(Time, 0.0f, false)
        {
        }
    }
}