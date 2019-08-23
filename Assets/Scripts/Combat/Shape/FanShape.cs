using LiteFramework.Helper;
using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class FanShape : CircleShape
    {
        public float BeginAngle { get; set; }
        public float EndAngle { get; set; }

        public FanShape(float Radius, float BeginAngle, float EndAngle)
            : base(Radius)
        {
            this.BeginAngle = BeginAngle;
            this.EndAngle = EndAngle;
        }

        public override bool Contains(Vector2 Center, Vector2 Position, Quaternion Rotation)
        {
            if (!base.Contains(Center, Position, Rotation))
            {
                return false;
            }

            var FixedPos = Quaternion.Inverse(Rotation) * (Position - Center);
            var Angle = MathHelper.GetAngle(Vector2.zero, FixedPos);

            if (BeginAngle < EndAngle)
            {
                return Angle >= BeginAngle && Angle <= EndAngle;
            }

            return Angle <= BeginAngle && Angle >= EndAngle;
        }
    }
}