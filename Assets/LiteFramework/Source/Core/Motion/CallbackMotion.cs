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

    public class CallbackMotion<T> : BaseMotion
    {
        private Action<T> Callback_;
        private T Param_;

        public CallbackMotion(Action<T> Callback, T Param)
            : base()
        {
            Callback_ = Callback;
            Param_ = Param;
            IsEnd = true;
        }

        public override void Enter()
        {
            Callback_?.Invoke(Param_);
        }

        public override void Exit()
        {
            Callback_ = null;
            Param_ = default;
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}