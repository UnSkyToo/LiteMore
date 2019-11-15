using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class SetAlphaMotion : BaseMotion
    {
        private readonly float Alpha_;
        private CanvasGroup Group_;
        private bool RecycleGroup_;

        public SetAlphaMotion(float Alpha)
        {
            Alpha_ = Alpha;
            IsEnd = true;
        }

        public override void Enter()
        {
            Group_ = Master.GetComponent<CanvasGroup>();
            RecycleGroup_ = false;
            if (Group_ == null)
            {
                Group_ = Master.gameObject.AddComponent<CanvasGroup>();
                RecycleGroup_ = true;
            }
            Group_.alpha = Alpha_;
        }

        public override void Exit()
        {
            if (RecycleGroup_ && Group_ != null)
            {
                Object.Destroy(Group_);
                Group_ = null;
                RecycleGroup_ = false;
            }
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}