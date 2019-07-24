using System;

namespace LiteMore.Combat.Npc
{
    public struct NpcAttrModifyInfo
    {
        public NpcAttrIndex Index { get; set; }
        public float Percent { get; set; }
        public float Base { get; set; }

        public NpcAttrModifyInfo(NpcAttrIndex Index, float Percent, float Base)
        {
            this.Index = Index;
            this.Percent = Percent;
            this.Base = Base;
        }
    }

    /// <summary>
    /// Final = Value * Percent + Base
    /// </summary>
    public class NpcAttribute
    {
        public event Action<NpcAttrIndex> AttrChanged; 

        private readonly float[] Value_;
        private readonly float[] Percent_;
        private readonly float[] Base_;

        public NpcAttribute(float[] Attrs)
        {
            Value_ = new float[(int)NpcAttrIndex.Count];
            Percent_ = new float[(int)NpcAttrIndex.Count];
            Base_ = new float[(int)NpcAttrIndex.Count];
            for (var Index = 0; Index < Value_.Length; ++Index)
            {
                if (Index >= Attrs.Length)
                {
                    Value_[Index] = 0;
                }
                else
                {
                    Value_[Index] = Attrs[Index];
                }

                Percent_[Index] = 1;
                Base_[Index] = 0;
            }
        }

        public float CalcFinalValue(NpcAttrIndex Index)
        {
            return GetValue(Index) * GetPercent(Index) + GetBase(Index);
        }

        public void SetValue(NpcAttrIndex Index, float Value, bool NotifyEvent = true)
        {
            Value_[(int)Index] = Value;
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Index);
            }
        }

        public void AddValue(NpcAttrIndex Index, float Value, bool NotifyEvent = true)
        {
            Value_[(int)Index] += Value;
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Index);
            }
        }

        public float GetValue(NpcAttrIndex Index)
        {
            return Value_[(int)Index];
        }

        public void ResetModify(NpcAttrIndex Index, bool NotifyEvent = true)
        {
            Percent_[(int)Index] = 1;
            Base_[(int)Index] = 0;
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Index);
            }
        }

        public void ApplyModify(NpcAttrModifyInfo Info, bool NotifyEvent = true)
        {
            ApplyPercent(Info.Index, Info.Percent, false);
            ApplyBase(Info.Index, Info.Base, false);
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Info.Index);
            }
        }

        public void RestoreModify(NpcAttrModifyInfo Info, bool NotifyEvent = true)
        {
            ApplyPercent(Info.Index, 1.0f / Info.Percent, false);
            ApplyBase(Info.Index, -Info.Base, false);
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Info.Index);
            }
        }

        /// <summary>
        /// 百分比乘法叠加
        /// </summary>
        public void ApplyPercent(NpcAttrIndex Index, float Percent, bool NotifyEvent = true)
        {
            Percent_[(int)Index] *= Percent;
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Index);
            }
        }

        public float GetPercent(NpcAttrIndex Index)
        {
            return Percent_[(int)Index];
        }

        /// <summary>
        /// 固定值加法叠加
        /// </summary>
        public void ApplyBase(NpcAttrIndex Index, float Value, bool NotifyEvent = true)
        {
            Base_[(int)Index] += Value;
            if (NotifyEvent)
            {
                AttrChanged?.Invoke(Index);
            }
        }

        public float GetBase(NpcAttrIndex Index)
        {
            return Base_[(int)Index];
        }
    }
}