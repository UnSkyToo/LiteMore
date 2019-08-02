using LiteMore.Extend;

namespace LiteMore.Combat.Skill.Selector
{
    public class ClickSelector : BaseSelector
    {
        public ClickSelector(MainSkill Skill)
            : base(SelectorMode.Click, Skill)
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