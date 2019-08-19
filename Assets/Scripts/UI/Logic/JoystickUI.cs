using LiteFramework.Game.UI;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Skill.Selector;
using LiteMore.UI.Item;
using UnityEngine;

namespace LiteMore.UI.Logic
{
    public class JoystickUI : BaseUI
    {
        public delegate void JoystickMoveEventDelegate(bool IsStop, Vector2 Direction, float Strength);
        public event JoystickMoveEventDelegate OnJoystickMoveEvent;

        public const float MinimumTouchDistance = 5;

        private RectTransform Slider_;
        private RectTransform CancelObj_;
        private float TraySize_;
        private bool IsMove_;
        private Vector2 TouchPosition_;
        private Vector2 PreviousPos_;

        private readonly BaseSelector[] SelectorList_;
        private readonly SkillIconItem[] SkillIconList_;

        public JoystickUI()
        {
            SelectorList_ = new BaseSelector[4];
            SkillIconList_ = new SkillIconItem[4];
        }

        protected override void OnOpen(params object[] Params)
        {
            Slider_ = FindComponent<RectTransform>("Tray/Gab");
            TraySize_ = FindComponent<RectTransform>("Tray").sizeDelta.x / 2.0f - MinimumTouchDistance;

            CancelObj_ = FindComponent<RectTransform>("Skill/Cancel");
            CancelObj_.gameObject.SetActive(false);

            AddEventToChild("Tray", OnTrayBeginDrag, UIEventType.BeginDrag);
            AddEventToChild("Tray", OnTrayDrag, UIEventType.Drag);
            AddEventToChild("Tray", OnTrayEndDrag, UIEventType.EndDrag);

            IsMove_ = false;
        }

        protected override void OnTick(float DeltaTime)
        {
            base.OnTick(DeltaTime);

            foreach (var Item in SkillIconList_)
            {
                Item?.Tick(DeltaTime);
            }
        }

        protected override void OnClose()
        {
            for (var Index = 0u; Index < 4; ++Index)
            {
                RemoveIcon(Index);
            }
        }

        private void RemoveIcon(uint Index)
        {
            if (Index >= 4 || SkillIconList_[Index] == null)
            {
                return;
            }

            SkillIconList_[Index].Dispose();
            SkillIconList_[Index] = null;
        }

        public void BindSkill(uint Index, BaseSkill Skill)
        {
            if (Index >= 4 || Skill == null)
            {
                return;
            }

            if (SelectorList_[Index] != null)
            {
                RemoveIcon(Index);
                SelectorList_[Index].Dispose();
                SelectorList_[Index] = null;
            }

            SkillIconList_[Index] = new SkillIconItem(FindChild($"Skill/Skill{Index+1}"), Skill, CancelObj_, false);
            SkillIconList_[Index].SetScaleToSize(FindComponent<RectTransform>($"Skill/Skill{Index+1}").sizeDelta);
        }

        private void OnTrayBeginDrag(GameObject Sender, Vector2 Pos)
        {
            IsMove_ = true;
            TouchPosition_ = Pos;
            PreviousPos_ = TouchPosition_;
        }

        private void OnTrayDrag(GameObject Sender, Vector2 Pos)
        {
            if (!IsMove_)
            {
                return;
            }

            if (Mathf.Abs(Pos.magnitude - PreviousPos_.magnitude) >= MinimumTouchDistance)
            {
                PreviousPos_ = Pos;
                var Offset = Pos - TouchPosition_;
                var Len = Mathf.Clamp(Offset.magnitude, -TraySize_, TraySize_);
                Slider_.anchoredPosition = (Offset.normalized * Len);
                OnJoystickMoveEvent?.Invoke(false, Offset.normalized, Len / TraySize_);
            }
        }

        private void OnTrayEndDrag(GameObject Sender, Vector2 Pos)
        {
            Slider_.anchoredPosition = Vector2.zero;
            IsMove_ = false;
            OnJoystickMoveEvent?.Invoke(true, Vector2.zero, 0);
        }
    }
}