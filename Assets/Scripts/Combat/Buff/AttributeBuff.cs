using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Buff
{
    public class AttributeBuffDescriptor : BaseBuffDescriptor
    {
        public NpcAttrModifyInfo Modify { get; }
        public int MaxTriggerCount { get; }

        public AttributeBuffDescriptor(string Name, float Duration, float Interval, float WaitTime, bool IsRefund, NpcAttrModifyInfo Modify, int MaxTriggerCount)
            : base(Name, Duration, Interval, WaitTime, IsRefund)
        {
            this.Modify = Modify;
            this.MaxTriggerCount = MaxTriggerCount;
        }
    }

    public class AttributeBuff : BaseBuff
    {
        private readonly uint TargetID_;
        private readonly NpcAttrModifyInfo Modify_;
        protected readonly int MaxTriggerCount_;
        protected int CurTriggerCount_;

        public AttributeBuff(AttributeBuffDescriptor Desc, uint TargetID)
            : base(BuffType.Attribute, Desc)
        {
            this.TargetID_ = TargetID;
            this.Modify_ = Desc.Modify;
            this.MaxTriggerCount_ = Desc.MaxTriggerCount;
            this.CurTriggerCount_ = 0;
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
            if (!IsRefund_)
            {
                return;
            }

            var Master = NpcManager.FindNpc(TargetID_);
            if (Master == null || !Master.IsValid())
            {
                return;
            }

            for (var Index = 0; Index < CurTriggerCount_; ++Index)
            {
                Master.Data.Attr.RestoreModify(Modify_);
            }
        }

        protected override void OnTrigger()
        {
            var Master = NpcManager.FindNpc(TargetID_);
            if (Master == null || !Master.IsValid())
            {
                return;
            }

            if (MaxTriggerCount_ > 0 && CurTriggerCount_ >= MaxTriggerCount_)
            {
                return;
            }

            CurTriggerCount_++;
            Master.Data.Attr.ApplyModify(Modify_);
        }
    }
}