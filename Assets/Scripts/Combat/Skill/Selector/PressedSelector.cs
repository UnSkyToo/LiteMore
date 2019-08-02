using LiteMore.Extend;
using LiteMore.Player;

namespace LiteMore.Combat.Skill.Selector
{
    public class PressedSelector : BaseSelector
    {
        protected bool IsPressed_;

        public PressedSelector(MainSkill Skill)
            : base(SelectorMode.Pressed, Skill)
        {
            IsPressed_ = false;
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Down, () => { IsPressed_ = true; });
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Up, () => { IsPressed_ = false; });
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Remove(Skill_.IconTransform);
        }

        public override void Recreated()
        {
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsPressed_)
            {
                if (PlayerManager.Player.Mp < Skill_.Cost)
                {
                    IsPressed_ = false;
                    return;
                }

                Skill_.Use(null);
            }
        }
    }
}