using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class CircleShape : BaseShape
    {
        public float Radius { get; set; }

        public CircleShape(float Radius)
        {
            this.Radius = Radius;
        }

        public override bool Contains(Vector2 Center, Vector2 Position, Quaternion Rotation)
        {
            return Vector2.Distance(Center, Position) <= Radius;
        }
    }
}