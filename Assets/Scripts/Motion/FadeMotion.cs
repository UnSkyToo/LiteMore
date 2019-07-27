using UnityEngine;

namespace LiteMore.Motion
{
    public class FadeMotion : BaseMotion
    {
        private readonly float TotalTime_;
        private readonly float BeginAlpha_;
        private readonly float EndAlpha_;
        private float CurrentTime_;

        private CanvasGroup Group_;

        public FadeMotion(float Time, float BeginAlpha, float EndAlpha)
            : base()
        {
            TotalTime_ = Time;
            BeginAlpha_ = BeginAlpha;
            EndAlpha_ = EndAlpha;
            CurrentTime_ = 0;
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

            Group_.alpha = BeginAlpha_;
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
            : base(Time, 0, 1)
        {
        }
    }

    public class FadeOutMotion : FadeMotion
    {
        public FadeOutMotion(float Time)
            : base(Time, 1, 0)
        {
        }
    }
}