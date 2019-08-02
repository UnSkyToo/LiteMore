using LiteMore.Core;

namespace LiteMore.Combat.Skill.Selector
{
    public enum SelectorMode : byte
    {
        Click, // 点击释放
        Pressed, // 持续按下释放
        DragPosition, // 拖动选择位置释放
        DragDirection, // 拖动选择方向释放
    }

    public abstract class BaseSelector : BaseEntity
    {
        public SelectorMode Mode { get; }
        protected MainSkill Skill_;

        protected BaseSelector(SelectorMode Mode)
            : base($"Selector {Mode}")
        {
            this.Mode = Mode;
        }

        public void BindSkill(MainSkill Skill)
        {
            Skill_ = Skill;
            OnBindSkill();
        }

        protected abstract void OnBindSkill();

        public override void Dispose()
        {
        }

        public abstract void Recreated();

        public override void Tick(float DeltaTime)
        {
        }
    }
}