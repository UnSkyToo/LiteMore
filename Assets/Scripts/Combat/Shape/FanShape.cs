using LiteFramework.Helper;
using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class FanShape : CircleShape
    {
        public float BeginAngle { get; set; }
        public float EndAngle { get; set; }

        public FanShape(Vector2 Center, float Radius, float BeginAngle, float EndAngle)
            : base(Center, Radius)
        {
            this.BeginAngle = BeginAngle;
            this.EndAngle = EndAngle;
        }

        public override bool Contains(Vector2 Position)
        {
            if (!base.Contains(Position))
            {
                return false;
            }

            var Angle = MathHelper.GetAngle(Center, Position);

            if (BeginAngle < EndAngle)
            {
                return Angle >= BeginAngle && Angle <= EndAngle;
            }

            return Angle <= BeginAngle && Angle >= EndAngle;
        }
    }
}