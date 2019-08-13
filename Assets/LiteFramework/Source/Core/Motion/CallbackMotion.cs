using System;

namespace LiteFramework.Core.Motion
{
    public class CallbackMotion : BaseMotion
    {
        private readonly Action Callback_;

        public CallbackMotion(Action Callback)
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