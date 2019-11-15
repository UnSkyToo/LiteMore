using LiteFramework.Game.EventSystem;
using LiteFramework.Game.UI;
using LiteFramework.Helper;

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
            EventHelper.AddEvent(Carrier_, OnPointerDown, EventSystemType.Down);
            EventHelper.AddEvent(Carrier_, OnPointerUp, EventSystemType.Up);
        }

        public override void Dispose()
        {
            EventHelper.RemoveEvent(Carrier_, OnPointerDown, EventSystemType.Down);
            EventHelper.RemoveEvent(Carrier_, OnPointerUp, EventSystemType.Up);
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