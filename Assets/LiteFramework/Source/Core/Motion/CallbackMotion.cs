using System;

namespace LiteFramework.Core.Motion
{
    public class CallbackMotion : BaseMotion
    {
        private Action Callback_;

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
            Callback_ = null;
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}