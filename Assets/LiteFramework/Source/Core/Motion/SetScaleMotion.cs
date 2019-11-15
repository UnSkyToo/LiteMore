using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class SetScaleMotion : BaseMotion
    {
        private readonly Vector2 Scale_;

        public SetScaleMotion(Vector2 Scale)
        {
            Scale_ = Scale;
            IsEnd = true;
        }

        public override void Enter()
        {
            Master.localScale = Scale_;
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}