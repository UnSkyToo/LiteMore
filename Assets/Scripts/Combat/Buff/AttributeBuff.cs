using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Buff
{
    public class AttributeBuffDescriptor : BaseBuffDescriptor
    {
        public uint TargetID { get; }
        public List<NpcAttrModifyInfo> ModifyList { get; }
        public int MaxTriggerCount { get; }
        public bool IsRefund { get; }

        public AttributeBuffDescriptor(string Name, float Duration, float Interval, float WaitTime, uint TargetID, List<NpcAttrModifyInfo> ModifyList, int MaxTriggerCount, bool IsRefund)
            : base(Name, Duration, Interval, WaitTime)
        {
            this.TargetID = TargetID;
            this.ModifyList = ModifyList;
            this.MaxTriggerCount = MaxTriggerCount;
            this.IsRefund = IsRefund;
        }
    }

    public class AttributeBuff : BaseBuff
    {
        private readonly uint TargetID_;
        private readonly List<NpcAttrModifyInfo> ModifyList_;
        private readonly int MaxTriggerCount_;
        private readonly bool IsRefund_;
        private int TotalTriggerCount_;

        public AttributeBuff(AttributeBuffDescriptor Desc)
            : base(BuffType.Attribute, Desc)
        {
            this.TargetID_ = Desc.TargetID;
            this.ModifyList_ = Desc.ModifyList;
            this.MaxTriggerCount_ = Desc.MaxTriggerCount;
            this.IsRefund_ = Desc.IsRefund;
            this.TotalTriggerCount_ = 0;
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
            if (IsRefund_)
            {
                var Master = NpcManager.FindNpc(TargetID_);
                if (Master == null || !Master.IsValid())
                {
                    return;
                }

                for (var Index = 0; Index < TotalTriggerCount_; ++Index)
                {
                    foreach (var Modify in ModifyList_)
                    {
                        Master.Data.Attr.RestoreModify(Modify);
                    }
                }
            }
        }

        protected override void OnTrigger()
        {
            var Master = NpcManager.FindNpc(TargetID_);
            if (Master == null || !Master.IsValid())
            {
                return;
            }

            if (MaxTriggerCount_ > 0 && TotalTriggerCount_ >= MaxTriggerCount_)
            {
                return;
            }

            TotalTriggerCount_++;
            foreach (var Modify in ModifyList_)
            {
                Master.Data.Attr.ApplyModify(Modify);
            }
        }
    }
}