using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public enum SelectorMode : byte
    {
        Click, // 点击释放
        Pressed, // 持续按下释放
        DragPosition, // 拖动选择位置释放
        DragDirection, // 拖动选择方向释放
    }

    public abstract class BaseSelector
    {
        public SelectorMode Mode { get; }
        protected readonly MainSkill Skill_;

        protected BaseSelector(SelectorMode Mode, MainSkill Skill)
        {
            this.Mode = Mode;
            this.Skill_ = Skill;
            this.Skill_.SetSelector(this);
        }

        public abstract void Destroy();

        public virtual void Tick(float DeltaTime)
        {
        }
    }
}