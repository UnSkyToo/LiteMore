using System.Collections.Generic;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Buff
{
    public static class BuffLibrary
    {
        private static readonly Dictionary<uint, BaseBuffDescriptor> BuffList_ = new Dictionary<uint, BaseBuffDescriptor>();

        public static void Generate()
        {
            BuffList_.Clear();
        }

        public static BaseBuffDescriptor Get(uint BuffID)
        {
            if (BuffList_.ContainsKey(BuffID))
            {
                return BuffList_[BuffID];
            }

            return null;
        }

        private static void AddAttr(uint BuffID, string Name, float Duration, float Interval, float WaitTime, bool IsRefund, NpcAttrModifyInfo Modify, int MaxTriggerCount)
        {
            if (BuffList_.ContainsKey(BuffID))
            {
                return;
            }

            BuffList_.Add(BuffID, new AttributeBuffDescriptor(Name, Duration, Interval, WaitTime, IsRefund, Modify, MaxTriggerCount));
        }

        private static void AddTrigger(uint BufferID, string Name, float Duration, float Interval, float WaitTime, bool IsRefund, NpcAttrModifyInfo Modify, float Radius, int MaxTriggerCount)
        {
            if (BuffList_.ContainsKey(BufferID))
            {
                return;
            }

            BuffList_.Add(BufferID, new TriggerBuffDescriptor(Name, Duration, Interval, WaitTime, IsRefund, Modify, Radius, MaxTriggerCount));
        }
    }
}