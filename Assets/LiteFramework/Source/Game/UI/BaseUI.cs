using System;
using LiteFramework.Core.Base;
using LiteFramework.Core.Motion;
using LiteFramework.Game.Asset;
using LiteFramework.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Game.UI
{
    public abstract class BaseUI : BaseObject
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Cached { get; set; }
        public Transform UITransform { get; set; }
        public RectTransform UIRectTransform { get; set; }
        public int DepthIndex { get; protected set; }
        public UIDepthMode DepthMode { get; protected set; }
        public bool IsClosed { get; private set; }

        private Canvas UICanvas_;
        public Canvas UICanvas
        {
            get
            {
                if (UICanvas_ != null)
                {
                    return UICanvas_;
                }

                UICanvas_ = UnityHelper.GetOrAddComponentSafe<Canvas>(UITransform.gameObject);
                UICanvas_.overrideSorting = true;
                UnityHelper.GetOrAddComponentSafe<GraphicRaycaster>(UITransform.gameObject);
                return UICanvas_;
            }
        }

        protected BaseUI(UIDepthMode Mode, int Index)
        {
            Name = GetType().Name;
            Cached = true;
            UITransform = null;
            UIRectTransform = null;
            DepthMode = Mode;
            DepthIndex = Index;
            IsClosed = false;
        }

        public virtual void Open(params object[] Params)
        {
            IsClosed = false;
            OnOpen(Params);
            Show();
        }

        public virtual void Close()
        {
            IsClosed = true;
            Hide();
            UIHelper.RemoveAllEvent(UITransform, true);
            OnClose();
        }

        public virtual void Show()
        {
            OnShow();
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(true);
            }
        }

        public virtual void Hide()
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

        public T GetComponent<T>(string ChildPath) where T : Component
        {
            return UIHelper.GetComponent<T>(UITransform, ChildPath);
        }

        public Component GetComponent(string ChildParent, Type CType)
        {
            return UIHelper.GetComponent(UITransform, ChildParent, CType);
        }

        public Component GetComponent(string ChildParent, string CType)
        {
            return UIHelper.GetComponent(UITransform, ChildParent, CType);
        }

        public void AddEvent(Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEvent(UITransform, Callback, Type);
        }

        public void AddEvent(Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEvent(UITransform, Callback, Type);
        }

        public void AddClickEvent(Action Callback, AssetUri AudioUri)
        {
            UIHelper.AddClickEvent(UITransform, Callback, AudioUri);
        }

        public void RemoveEvent(Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEvent(UITransform, Callback, Type);
        }

        public void RemoveEvent(Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEvent(UITransform, Callback, Type);
        }

        public void AddEventToChild(string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEventToChild(UITransform, ChildPath, Callback, Type);
        }

        public void AddEventToChild(string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.AddEventToChild(UITransform, ChildPath, Callback, Type);
        }

        public void AddClickEventToChild(string ChildPath, Action Callback, AssetUri AudioUri)
        {
            UIHelper.AddClickEventToChild(UITransform, ChildPath, Callback, AudioUri);
        }

        public void RemoveEventFromChild(string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEventFromChild(UITransform, ChildPath, Callback, Type);
        }

        public void RemoveEventFromChild(string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIHelper.RemoveEventFromChild(UITransform, ChildPath, Callback, Type);
        }

        public void SetActive(bool Value)
        {
            UITransform.gameObject.SetActive(Value);
        }

        public void SetActive(string ChildPath, bool Value)
        {
            UIHelper.SetActive(UITransform, ChildPath, Value);
        }

        public void EnableTouched(bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, Enabled);
        }

        public void EnableTouched(string ChildPath, bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, ChildPath, Enabled);
        }

        public BaseMotion ExecuteMotion(BaseMotion Motion)
        {
            return MotionManager.Execute(UITransform, Motion);
        }

        public void AbandonMotion(BaseMotion Motion)
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