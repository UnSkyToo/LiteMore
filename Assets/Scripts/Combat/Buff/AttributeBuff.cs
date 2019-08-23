using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Buff
{
    public struct AttributeBuffDescriptor
    {
        public BaseBuffDescriptor BaseDesc { get; }
        public uint TargetID { get; }
        public List<NpcAttrModifyInfo> ModifyList { get; }

        public AttributeBuffDescriptor(BaseBuffDescriptor BaseDesc, uint TargetID, List<NpcAttrModifyInfo> ModifyList)
        {
            this.BaseDesc = BaseDesc;
            this.TargetID = TargetID;
            this.ModifyList = ModifyList;
        }
    }

    public class AttributeBuff : BaseBuff
    {
        private readonly uint TargetID_;
        private readonly List<NpcAttrModifyInfo> ModifyList_;
        private int TotalTriggerCount_;

        public AttributeBuff(AttributeBuffDescriptor Desc)
            : base(BuffType.Attribute, Desc.BaseDesc)
        {
            this.TargetID_ = Desc.TargetID;
            this.ModifyList_ = Desc.ModifyList;
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
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

        protected override void OnTrigger()
        {
            var Master = NpcManager.FindNpc(TargetID_);
            if (Master == null || !Master.IsValid())
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