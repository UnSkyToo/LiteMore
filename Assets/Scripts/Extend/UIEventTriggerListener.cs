using System;
using LiteMore.Helper;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LiteMore.Extend
{
    public enum UIEventType
    {
        Click = 0,
        Down = 1,
        Up = 2,
        Enter = 3,
        Exit = 4,
        Drag = 5,
        BeginDrag = 6,
        EndDrag = 7,
        Count = 8,
    }

    public class UIEventTriggerListener : EventTrigger
    {
        private readonly Action<UnityEngine.GameObject, Vector2>[] EventCallback_ = new Action<UnityEngine.GameObject, Vector2>[(int)UIEventType.Count];

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
            EventCallback_[(int)UIEventType.Click]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnPointerDown(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Down]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnPointerUp(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Up]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnPointerEnter(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Enter]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnPointerExit(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Exit]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnBeginDrag(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.BeginDrag]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnDrag(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Drag]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }

        public override void OnEndDrag(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.EndDrag]?.Invoke(EventData.pointerPress, UnityHelper.ScreenPosToCanvasPos(EventData.position));
        }
    }
}