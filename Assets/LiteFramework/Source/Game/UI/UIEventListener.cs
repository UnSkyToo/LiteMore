using System;
using System.Collections.Generic;
using LiteFramework.Game.UI.Event;
using UnityEngine;
using LiteFramework.Extend;

namespace LiteFramework.Game.UI
{
    public static class UIEventListener
    {
        private static readonly Dictionary<UIEventType, Type> EventHandlerList_ = new Dictionary<UIEventType, Type>
        {
            {UIEventType.Click, typeof(UIEventPointerClickHandler)},
            {UIEventType.Down, typeof(UIEventPointerDownHandler)},
            {UIEventType.Up, typeof(UIEventPointerUpHandler)},
            {UIEventType.Enter, typeof(UIEventPointerEnterHandler)},
            {UIEventType.Exit, typeof(UIEventPointerExitHandler)},
            {UIEventType.Drag, typeof(UIEventDragHandler)},
            {UIEventType.BeginDrag, typeof(UIEventBeginDragHandler)},
            {UIEventType.EndDrag, typeof(UIEventEndDragHandler)},
            {UIEventType.Cancel, typeof(UIEventCancelHandler)},
        };

        public static void AddCallback(Transform Master, UIEventType Type, Action<GameObject, Vector2> Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).AddCallback(Callback);
        }

        public static void AddCallback(Transform Master, UIEventType Type, Action Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).AddCallback(Callback);
        }

        public static void RemoveCallback(Transform Master, UIEventType Type, Action<GameObject, Vector2> Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).RemoveCallback(Callback);
        }

        public static void RemoveCallback(Transform Master, UIEventType Type, Action Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).RemoveCallback(Callback);
        }

        public static void ClearCallback(Transform Master)
        {
            if (Master.GetComponent(typeof(UIEventBaseHandler)) == null)
            {
                return;
            }

            for (var Index = 0; Index < EnumEx.Count<UIEventType>(); ++Index)
            {
                var Handler = Master.GetComponent(EventHandlerList_[(UIEventType)Index]) as UIEventBaseHandler;
                if (Handler != null)
                {
                    Handler.Dispose();
                    UnityEngine.Object.Destroy(Handler);
                }
            }
        }

        private static UIEventBaseHandler GetOrCreateHandler(GameObject Master, UIEventType Type)
        {
            var Handler = Master.GetComponent(EventHandlerList_[Type]);
            if (Handler == null)
            {
                Handler = Master.AddComponent(EventHandlerList_[Type]);
            }
            return Handler as UIEventBaseHandler;
        }
    }
}