using UnityEngine;

namespace LiteMore.Combat.Shape
{
    public abstract class BaseShape
    {
        public abstract bool Contains(Vector2 Position);
    }
}