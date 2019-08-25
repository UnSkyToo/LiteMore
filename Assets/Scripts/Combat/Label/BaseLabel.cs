using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Label
{
    public abstract class BaseLabel : GameEntity
    {
        protected BaseLabel(string Name, Transform Trans)
            : base(Name, Trans)
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}