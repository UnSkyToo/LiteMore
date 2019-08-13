using LiteFramework.Game.UI;

namespace LiteMore.Combat.Skill.Selector
{
    public class ClickSelector : BaseSelector
    {
        public ClickSelector()
            : base(SelectorMode.Click)
        {
        }

        protected override void OnBindSkill()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Click, OnClick);
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.Click, OnClick);
        }

        public override void Recreated()
        {
        }

        private void OnClick()
        {
            Skill_.Use(null);
        }
    }
}