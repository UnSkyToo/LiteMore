using System;
using LiteMore.Extend;
using LiteMore.Helper;
using LiteMore.Motion;
using UnityEngine;

namespace LiteMore.UI
{
    public enum UIDepthMode
    {
        Bottom = 0,
        Normal = 5000,
        Top = 10000
    }

    public class UIBase
    {
        public uint ID { get; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Transform UITransform { get; set; }
        public RectTransform UIRectTransform { get; set; }
        public int DepthIndex { get; protected set; }
        public UIDepthMode DepthMode { get; protected set; }

        public int SortOrder => DepthIndex + (int)DepthMode;

        public UIBase()
        {
            ID = IDGenerator.Get();
            Name = GetType().Name;
            UITransform = null;
            UIRectTransform = null;
            DepthIndex = 0;
            DepthMode = UIDepthMode.Normal;
        }

        public void Open(params object[] Params)
        {
            OnOpen(Params);
            Show();
        }

        public void Close()
        {
            Hide();
            UIHelper.RemoveAllEvent(UITransform, true);
            OnClose();
        }

        public void Show()
        {
            OnShow();
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            OnHide();
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(false);
            }
        }

        public void Tick(float DeltaTime)
        {
            OnTick(DeltaTime);
        }

        public Transform FindChild(string ChildPath)
        {
            return UIHelper.FindChild(UITransform, ChildPath);
        }

        public T FindComponent<T>(string ChildPath) where T : Component
        {
            return UIHelper.FindComponent<T>(UITransform, ChildPath);
        }

        public Component FindComponent(string ChildParent, Type CType)
        {
            return UIHelper.FindComponent(UITransform, ChildParent, CType);
        }

        public Component FindComponent(string ChildParent, string CType)
        {
            return UIHelper.FindComponent(UITransform, ChildParent, CType);
        }

        public void AddEvent(Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEvent(UITransform, Callback, Type);
        }

        public void RemoveEvent(Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEvent(UITransform, Callback, Type);
        }

        public void AddEventToChild(string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEventToChild(UITransform, ChildPath, Callback, Type);
        }

        public void RemoveEventFromChild(string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEventFromChild(UITransform, ChildPath, Callback, Type);
        }

        public void ShowChild(string ChildPath)
        {
            UIHelper.ShowChild(UITransform, ChildPath);
        }

        public void HideChild(string ChildPath)
        {
            UIHelper.HideChild(UITransform, ChildPath);
        }

        public void EnableTouched(bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, Enabled);
        }

        public void EnableTouched(string ChildPath, bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, ChildPath, Enabled);
        }

        public MotionBase ExecuteMotion(MotionBase Motion)
        {
            return MotionManager.Execute(UITransform, Motion);
        }

        public void AbandonMotion(MotionBase Motion)
        {
            MotionManager.Abandon(Motion);
        }

        protected virtual void OnOpen(params object[] Params)
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnTick(float DeltaTime)
        {
        }
    }
}