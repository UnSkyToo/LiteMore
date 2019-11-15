using LiteFramework.Game.EventSystem;
using LiteFramework.Game.UI;
using LiteFramework.Helper;

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
            EventHelper.AddEvent(Carrier_, OnClick);
        }

        public override void Dispose()
        {
            EventHelper.RemoveEvent(Carrier_, OnClick);
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