using LiteFramework.Game.UI;

namespace LiteMore.Combat.Skill.Selector
{
    public class PressedSelector : BaseSelector
    {
        protected bool IsPressed_;

        public PressedSelector()
            : base(SelectorMode.Pressed)
        {
            IsPressed_ = false;
        }

        public override BaseSelector Clone()
        {
            return new PressedSelector();
        }

        protected override void OnBindCarrier()
        {
            UIEventListener.AddCallback(Carrier_, UIEventType.Down, OnPointerDown);
            UIEventListener.AddCallback(Carrier_, UIEventType.Up, OnPointerUp);
        }

        public override void Dispose()
        {
            UIEventListener.RemoveCallback(Carrier_, UIEventType.Down, OnPointerDown);
            UIEventListener.RemoveCallback(Carrier_, UIEventType.Up, OnPointerUp);
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsPressed_)
            {
                if (!CanUse())
                {
                    IsPressed_ = false;
                    return;
                }

                Used();
            }
        }

        private void OnPointerDown()
        {
            IsPressed_ = true;
        }

        private void OnPointerUp()
        {
            IsPressed_ = false;
        }
    }
}