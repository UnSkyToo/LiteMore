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
            UIEventListener.AddCallback(Carrier_, UIEventType.Click, OnClick);
        }

        public override void Dispose()
        {
            UIEventListener.RemoveCallback(Carrier_, UIEventType.Click, OnClick);
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