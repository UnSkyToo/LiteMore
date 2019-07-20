using System;

namespace LiteMore.Motion
{
    public class MotionCallback : MotionBase
    {
        private readonly Action Callback_;

        public MotionCallback(Action Callback)
            : base()
        {
            Callback_ = Callback;
            IsEnd = true;
        }

        public override void Enter()
        {
            Callback_?.Invoke();
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}