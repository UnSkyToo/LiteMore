using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class SetPositionMotion : BaseMotion
    {
        private readonly Vector2 Position_;

        public SetPositionMotion(Vector2 Position)
        {
            Position_ = Position;
            IsEnd = true;
        }

        public override void Enter()
        {
            Master.localPosition = Position_;
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}