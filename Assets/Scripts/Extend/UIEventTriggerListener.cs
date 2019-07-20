using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LiteMore.Extend
{
    public enum UIEventType
    {
        Click = 0,
        Down = 1,
        Up = 2,
        Count = 3,
    }

    public class UIEventTriggerListener : EventTrigger
    {
        private readonly Action<UnityEngine.GameObject, Vector2>[] EventCallback_ = new Action<UnityEngine.GameObject, Vector2>[(int)UIEventType.Count];
        private static readonly RectTransform RootCanvas_ = GameObject.Find("Canvas").GetComponent<RectTransform>();

        public static UIEventTriggerListener Get(UnityEngine.Transform Obj)
        {
            var Listener = Obj.GetComponent<UIEventTriggerListener>();
            if (Listener == null)
            {
                Listener = Obj.gameObject.AddComponent<UIEventTriggerListener>();
            }

            Obj.GetComponent<UnityEngine.UI.Graphic>().raycastTarget = true;
            return Listener;
        }

        public static void Remove(UnityEngine.Transform Obj)
        {
            var Listener = Obj.GetComponent<UIEventTriggerListener>();
            if (Listener != null)
            {
                Obj.GetComponent<UnityEngine.UI.Graphic>().raycastTarget = false;
                UnityEngine.Object.DestroyImmediate(Listener);
            }
        }

        public void AddCallback(UIEventType Type, Action<UnityEngine.GameObject, Vector2> Callback)
        {
            EventCallback_[(int)Type] += Callback;
        }

        public void RemoveCallback(UIEventType Type, Action<UnityEngine.GameObject, Vector2> Callback)
        {
            if (Callback == null)
            {
                EventCallback_[(int)Type] = null;
            }
            else
            {
                EventCallback_[(int)Type] -= Callback;
            }
        }

        public override void OnPointerClick(PointerEventData EventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas_, EventData.position, Camera.main, out Vector2 Pos);
            EventCallback_[(int)UIEventType.Click]?.Invoke(EventData.pointerPress, Pos);
        }

        public override void OnPointerDown(PointerEventData EventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas_, EventData.position, Camera.main, out Vector2 Pos);
            EventCallback_[(int)UIEventType.Down]?.Invoke(EventData.pointerPress, Pos);
        }

        public override void OnPointerUp(PointerEventData EventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas_, EventData.position, Camera.main, out Vector2 Pos);
            EventCallback_[(int)UIEventType.Up]?.Invoke(EventData.pointerPress, Pos);
        }
    }
}