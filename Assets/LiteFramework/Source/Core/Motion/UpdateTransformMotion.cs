using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class UpdateTransformMotion : BaseMotion
    {
        private readonly Vector2? Position_;
        private readonly Vector2? Scale_;
        private readonly Quaternion? Rotation_;

        public UpdateTransformMotion(Vector2 Position)
            : base()
        {
            Position_ = Position;
            Scale_ = null;
            Rotation_ = null;
            IsEnd = true;
        }

        public UpdateTransformMotion(Vector2 Position, Vector2 Scale)
            : base()
        {
            Position_ = Position;
            Scale_ = Scale;
            Rotation_ = null;
            IsEnd = true;
        }

        public UpdateTransformMotion(Vector2 Position, Vector2 Scale, Quaternion Rotation)
            : base()
        {
            Position_ = Position;
            Scale_ = Scale;
            Rotation_ = Rotation;
            IsEnd = true;
        }

        public override void Enter()
        {
            if (Position_ != null)
            {
                Master.localPosition = Position_.Value;
            }

            if (Scale_ != null)
            {
                Master.localScale = Scale_.Value;
            }

            if (Rotation_ != null)
            {
                Master.localRotation = Rotation_.Value;
            }
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}