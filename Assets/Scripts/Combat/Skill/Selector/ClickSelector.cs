using LiteFramework.Game.UI;

namespace LiteMore.Combat.Skill.Selector
{
    public class ClickSelector : BaseSelector
    {
        public ClickSelector()
            : base(SelectorMode.Click)
        {
        }

        public override BaseSelector Clone()
        {
            return new ClickSelector();
        }

        protected override void OnBindCarrier()
        {
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.Click, OnClick);
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Get(Carrier_).RemoveCallback(UIEventType.Click, OnClick);
        }

        private void OnClick()
        {
            if (!CanUse())
            {
                return;
            }

            Used();
        }
    }
}