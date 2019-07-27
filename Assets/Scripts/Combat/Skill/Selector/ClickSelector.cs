using LiteMore.Extend;

namespace LiteMore.Combat.Skill.Selector
{
    public class ClickSelector : BaseSelector
    {
        public event System.Action<MainSkill> OnUsed;

        public ClickSelector(MainSkill Skill)
            : base(SelectorMode.Click, Skill)
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Click, OnClick);
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.Click, OnClick);
        }

        private void OnClick()
        {
            Use();
        }

        private void Use()
        {
            if (!Skill_.Use())
            {
                return;
            }

            OnUsed?.Invoke(Skill_);
        }
    }
}