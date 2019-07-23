using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class RectShape : BaseShape
    {
        public Vector2 Center { get; set; }

        private Vector2 Size_;
        private Vector2 HalfSize_;

        public Vector2 Size
        {
            get => Size_;
            set
            {
                Size_ = value;
                HalfSize_ = Size_ / 2;
            }
        }

        private Quaternion Rotation_;
        private Quaternion InvertRotation_;

        public Quaternion Rotation
        {
            get => Rotation_;
            set
            {
                Rotation_ = value;
                InvertRotation_ = Quaternion.Inverse(value);
            }
        }

        public RectShape(Vector2 Center, Vector2 Size)
            : this(Center, Size, Quaternion.identity)
        {
        }

        public RectShape(Vector2 Center, Vector2 Size, Quaternion Rotation)
        {
            this.Center = Center;
            this.Size = Size;
            this.Rotation = Rotation;
        }

        public override bool Contains(Vector2 Position)
        {
            var TargetPos = InvertRotation_ * (Position - Center);
            if (TargetPos.x < -HalfSize_.x || TargetPos.x > HalfSize_.x ||
                TargetPos.y < -HalfSize_.y || TargetPos.y > HalfSize_.y)
            {
                return false;
            }

            return true;
        }
    }
}