using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class SetRotationMotion : BaseMotion
    {
        private readonly Quaternion Rotation_;

        public SetRotationMotion(Quaternion Rotation)
        {
            Rotation_ = Rotation;
            IsEnd = true;
        }

        public override void Enter()
        {
            Master.localRotation = Rotation_;
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}