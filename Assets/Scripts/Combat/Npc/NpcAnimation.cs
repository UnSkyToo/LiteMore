using System;
using System.Collections.Generic;
using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class NpcAnimation
    {
        private struct NpcAnimationMsg
        {
            internal uint ID { get; }

            private readonly string Animation_;
            private readonly float Percent_;
            private readonly CombatMsgCode MsgCode_;
            private readonly Action<string, CombatMsgCode> Callback_;

            internal NpcAnimationMsg(string Animation, float Percent, CombatMsgCode MsgCode, Action<string, CombatMsgCode> Callback)
            {
                this.ID = IDGenerator.Get();
                this.Animation_ = Animation;
                this.Percent_ = Mathf.Clamp01(Percent);
                this.MsgCode_ = MsgCode;
                this.Callback_ = Callback;
            }

            internal void Invoke(string CurrentAnimation, float PreviousTime, float CurrentTime)
            {
                if (CurrentAnimation != Animation_)
                {
                    return;
                }

                if (PreviousTime <= CurrentTime && PreviousTime <= Percent_ && Percent_ <= CurrentTime)
                {
                    Callback_?.Invoke(Animation_, MsgCode_);
                }
                else if (PreviousTime > CurrentTime && PreviousTime < Percent_)
                {
                    Callback_?.Invoke(Animation_, MsgCode_);
                }
            }
        }

        private readonly Animator Animator_;

        private string CurrentAnimName_;
        private bool CurrentAnimLoop_;

        private float PreviousTime_;
        private float CurrentTime_;

        private readonly List<NpcAnimationMsg> MsgList_;

        public NpcAnimation(Animator Ani)
        {
            this.Animator_ = Ani;

            this.CurrentAnimName_ = string.Empty;
            this.CurrentAnimLoop_ = false;

            this.PreviousTime_ = 0;
            this.CurrentTime_ = 0;
            
            this.MsgList_ = new List<NpcAnimationMsg>();
        }

        public void Tick(float DeltaTime)
        {
            PreviousTime_ = CurrentTime_;
            CurrentTime_ = GetAnimationTime() % 1.0f;

            foreach (var Msg in MsgList_)
            {
                Msg.Invoke(CurrentAnimName_, PreviousTime_, CurrentTime_);
            }
        }

        public uint RegisterMsg(string Animation, float Percent, CombatMsgCode MsgCode, Action<string, CombatMsgCode> Callback)
        {
            var Msg = new NpcAnimationMsg(Animation, Percent, MsgCode, Callback);
            MsgList_.Add(Msg);
            return Msg.ID;
        }

        public void UnRegisterMsg(uint ID)
        {
            for (var Index = 0; Index < MsgList_.Count; ++Index)
            {
                if (MsgList_[Index].ID == ID)
                {
                    MsgList_.RemoveAt(Index);
                    return;
                }
            }
        }

        public void Play(string AnimName, bool IsLoop)
        {
            if (CurrentAnimName_ == AnimName && IsLoop == CurrentAnimLoop_)
            {
                return;
            }

            if (!string.IsNullOrEmpty(CurrentAnimName_))
            {
                Animator_.ResetTrigger(CurrentAnimName_);
            }

            CurrentAnimName_ = AnimName;
            CurrentAnimLoop_ = IsLoop;
            PreviousTime_ = 0;
            CurrentTime_ = 0;
            Animator_.SetTrigger(CurrentAnimName_);
        }

        public bool IsEnd()
        {
            if (CurrentAnimLoop_)
            {
                return false;
            }

            return GetAnimationTime() >= 1.0f;
        }

        private float GetAnimationTime()
        {
            var Info = Animator_.GetCurrentAnimatorStateInfo(0);
            if (!Info.IsTag(CurrentAnimName_))
            {
                return 0;
            }

            return Info.normalizedTime;
        }
    }
}