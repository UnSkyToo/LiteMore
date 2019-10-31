namespace LiteFramework.Core.Motion
{
    public class ActiveMotion : BaseMotion
    {
        private readonly bool Value_;

        public ActiveMotion(bool Value)
            : base()
        {
            Value_ = Value;
            IsEnd = true;
        }

        public override void Enter()
        {
            Master?.gameObject.SetActive(Value_);
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}