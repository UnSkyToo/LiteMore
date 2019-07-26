using LiteMore.Extend;

namespace LiteMore.Combat.Skill.Selector
{
    public class PressedSelector : BaseSelector
    {
        public event System.Action<MainSkill> OnUsed;
        protected readonly float Interval_;
        protected float Time_;
        protected bool IsPressed_;

        public PressedSelector(MainSkill Skill, float Interval)
            : base(SelectorMode.Pressed, Skill)
        {
            Interval_ = Interval;
            Time_ = 0;
            IsPressed_ = false;
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Down, () => { IsPressed_ = true; Time_ = 0;});
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Up, () => { IsPressed_ = false; Time_ = 0;});
        }

        public override void Destroy()
        {
            UIEventTriggerListener.Remove(Skill_.IconTransform);
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsPressed_)
            {
                if (PlayerManager.Mp < Skill_.Cost)
                {
                    IsPressed_ = false;
                    return;
                }

                Time_ += DeltaTime;
                if (Time_ >= Interval_)
                {
                    Time_ -= Interval_;
                    Use();
                }
            }
        }

        private void Use()
        {
            /*if (!Skill_.Use())
            {
                return;
            }*/
            PlayerManager.AddMp(-Skill_.Cost);
            OnUsed?.Invoke(Skill_);
        }
    }
}