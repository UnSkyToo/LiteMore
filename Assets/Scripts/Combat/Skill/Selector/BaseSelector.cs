using System;
using System.Collections.Generic;
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
        public event Action<Dictionary<string, object>> OnUsed; 

        public SelectorMode Mode { get; }
        protected Transform Carrier_;
        private Func<bool> CanUseFunc_;

        protected BaseSelector(SelectorMode Mode)
            : base($"Selector {Mode}")
        {
            this.Mode = Mode;
        }

        public void BindCarrier(Transform Carrier, Func<bool> CanUseFunc, Dictionary<string, object> Args)
        {
            if (Carrier_ != null)
            {
                Dispose();
            }

            Carrier_ = Carrier;
            CanUseFunc_ = CanUseFunc;
            OnBindCarrier(Args);
        }

        protected abstract void OnBindCarrier(Dictionary<string, object> Args);

        public override void Dispose()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }

        protected bool CanUse()
        {
            return !(CanUseFunc_ != null && CanUseFunc_?.Invoke() == false);
        }

        protected void Used(Dictionary<string, object> Args)
        {
            OnUsed?.Invoke(Args);
        }
    }
}