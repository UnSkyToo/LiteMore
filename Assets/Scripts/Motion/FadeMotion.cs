using UnityEngine;

namespace LiteMore.Motion
{
    public class FadeMotion : BaseMotion
    {
        private readonly bool IsRelative_;
        private readonly float TotalTime_;
        private float CurrentTime_;
        private float BeginAlpha_;
        private float EndAlpha_;

        private CanvasGroup Group_;

        public FadeMotion(float Time, float Alpha, bool IsRelative)
            : base()
        {
            IsRelative_ = IsRelative;
            TotalTime_ = Time;
            BeginAlpha_ = Alpha;
            EndAlpha_ = Alpha;
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

    public class FadeInMotion : FadeMotion
    {
        public FadeInMotion(float Time)
            : base(Time, 1.0f, false)
        {
        }
    }

    public class FadeOutMotion : FadeMotion
    {
        public FadeOutMotion(float Time)
            : base(Time, 0.0f, false)
        {
        }
    }
}