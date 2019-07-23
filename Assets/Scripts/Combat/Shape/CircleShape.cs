using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public class CircleShape : BaseShape
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public CircleShape(Vector2 Center, float Radius)
        {
            this.Center = Center;
            this.Radius = Radius;
        }

        public override bool Contains(Vector2 Position)
        {
            return Vector2.Distance(Center, Position) <= Radius;
        }
    }
}