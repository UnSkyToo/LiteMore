using System;
using LiteMore.Core;
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

    public abstract class BaseSelector : BaseEntity
    {
        public SelectorMode Mode { get; }
        protected Transform Carrier_;
        protected SkillArgs Args_;
        protected Action<SkillArgs> OnUsed_;

        protected BaseSelector(SelectorMode Mode)
            : base($"Selector {Mode}")
        {
            this.Mode = Mode;
        }

        public abstract BaseSelector Clone();

        public void BindCarrier(Transform Carrier, SkillArgs Args, Action<SkillArgs> OnUsed)
        {
            if (Carrier_ != null)
            {
                Dispose();
            }

            Carrier_ = Carrier;
            Args_ = Args;
            OnUsed_ = OnUsed;
            OnBindCarrier();
        }

        protected abstract void OnBindCarrier();

        public override void Dispose()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }

        protected bool CanUse()
        {
            return Args_ != null && Args_.CanUse();
        }

        protected void Used()
        {
            OnUsed_.Invoke(Args_);
        }
    }
}