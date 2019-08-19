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
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.Down, () => { IsPressed_ = true; });
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.Up, () => { IsPressed_ = false; });
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Remove(Carrier_);
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
    }
}