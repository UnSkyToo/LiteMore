using System;
using System.Collections.Generic;
using LiteFramework.Game.UI;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Skill.Selector;
using UnityEngine;

namespace LiteMore.UI.Logic
{
    public class JoystickUI : BaseUI
    {
        public delegate void JoystickMoveEventDelegate(bool IsStop, Vector2 Direction, float Strength);
        public event JoystickMoveEventDelegate OnJoystickMoveEvent;

        public delegate void JoystickSkillEventDelegate(int Index, bool Cancel);
        public event JoystickSkillEventDelegate OnJoystickSkillEvent;

        public const float MinimumTouchDistance = 5;

        private RectTransform Slider_;
        private RectTransform CancelObj_;
        private float TraySize_;
        private bool IsMove_;
        private Vector3 TouchPosition_;
        private Vector3 PreviousPos_;

        private BaseSelector[] SelectorList_;
        private Transform[] SkillObjList_;

        public JoystickUI()
        {
            SelectorList_ = new BaseSelector[4];
        }

        protected override void OnOpen(params object[] Params)
        {
            Slider_ = FindComponent<RectTransform>("Tray/Gab");
            TraySize_ = FindComponent<RectTransform>("Tray").sizeDelta.x / 2.0f - MinimumTouchDistance;

            CancelObj_ = FindComponent<RectTransform>("Skill/Cancel");
            CancelObj_.gameObject.SetActive(false);

            SkillObjList_ = new Transform[4];
            for (var Index = 0; Index < 4; ++Index)
            {
                SkillObjList_[Index] = FindChild($"Skill/Skill{Index + 1}");
                SkillObjList_[Index].gameObject.SetActive(false);
            }

            AddEventToChild("Tray", OnTrayTouchUp, UIEventType.Up);
            AddEventToChild("Tray", OnTrayTouchDown, UIEventType.Down);

            IsMove_ = false;
        }

        protected override void OnTick(float DeltaTime)
        {
            base.OnTick(DeltaTime);

            if (IsMove_)
            {
                var CurrentPos = Vector3.zero;
                if (Input.touchSupported)
                {
                    CurrentPos = Input.touches[0].position;
                }
                else
                {
                    CurrentPos = Input.mousePosition;
                }

                if (Mathf.Abs(CurrentPos.magnitude - PreviousPos_.magnitude) >= MinimumTouchDistance)
                {
                    PreviousPos_ = CurrentPos;
                    var Offset = CurrentPos - TouchPosition_;
                    var Len = Mathf.Clamp(Offset.magnitude, -TraySize_, TraySize_);
                    Slider_.anchoredPosition = (Offset.normalized * Len);
                    OnJoystickMoveEvent?.Invoke(false, Offset.normalized, Len / TraySize_);
                }
            }
        }

        public void BindSkill(uint Index, BaseSelector Selector, BaseSkill Skill, Func<bool> CanUseFunc, Dictionary<string, object> Args)
        {
            if (Index >= 4 || Selector == null || Skill == null)
            {
                return;
            }

            if (SelectorList_[Index] != null)
            {
                SelectorList_[Index].Dispose();
                SelectorList_[Index] = null;
            }

            if (Args == null)
            {
                Args = new Dictionary<string, object>();
            }

            Args.Add("Radius", Skill.Radius);
            Args.Add("CancelObj", CancelObj_);

            SkillObjList_[Index].gameObject.SetActive(true);
            Selector.BindCarrier(SkillObjList_[Index], CanUseFunc, Args);
            //Selector.OnUsed += 
        }

        private void OnTrayTouchUp()
        {
            Slider_.anchoredPosition = Vector2.zero;
            IsMove_ = false;
            OnJoystickMoveEvent?.Invoke(true, Vector2.zero, 0);
        }

        private void OnTrayTouchDown()
        {
            IsMove_ = true;

            if (Input.touchSupported)
            {
                TouchPosition_ = Input.touches[0].position;
            }
            else
            {
                TouchPosition_ = Input.mousePosition;
            }

            PreviousPos_ = TouchPosition_;
        }
    }
}