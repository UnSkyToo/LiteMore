using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class RectShape : BaseShape
    {
        private readonly Vector2 HalfSize_;

        public RectShape(Vector2 Size)
        {
            HalfSize_ = Size / 2;
        }

        public override bool Contains(Vector2 Center, Vector2 Position, Quaternion Rotation)
        {
            var TargetPos = Quaternion.Inverse(Rotation) * (Position - Center);
            if (TargetPos.x < -HalfSize_.x || TargetPos.x > HalfSize_.x ||
                TargetPos.y < -HalfSize_.y || TargetPos.y > HalfSize_.y)
            {
                return false;
            }

            return true;
        }
    }
}