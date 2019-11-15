using System;
using System.Collections.Generic;
using UnityEngine;
using LiteFramework.Extend;
using UnityEngine.Events;

namespace LiteFramework.Game.EventSystem
{
    public static class EventSystemListener
    {
        private static readonly Dictionary<EventSystemType, Type> EventHandlerList_ = new Dictionary<EventSystemType, Type>
        {
            {EventSystemType.Click, typeof(EventSystemPointerClickHandler)},
            {EventSystemType.Down, typeof(EventSystemPointerDownHandler)},
            {EventSystemType.Up, typeof(EventSystemPointerUpHandler)},
            {EventSystemType.Enter, typeof(EventSystemPointerEnterHandler)},
            {EventSystemType.Exit, typeof(EventSystemPointerExitHandler)},
            {EventSystemType.Drag, typeof(EventSystemDragHandler)},
            {EventSystemType.BeginDrag, typeof(EventSystemBeginDragHandler)},
            {EventSystemType.EndDrag, typeof(EventSystemEndDragHandler)},
            {EventSystemType.Cancel, typeof(EventSystemCancelHandler)},
        };

        private static readonly int EventTypeCount_ = EnumEx.Count<EventSystemType>();

        public static void AddCallback(Transform Master, EventSystemType Type, Action<EventSystemData> Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).AddCallback(Callback);
        }

        public static void AddCallback(Transform Master, EventSystemType Type, UnityAction Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).AddCallback(Callback);
        }

        public static void RemoveCallback(Transform Master, EventSystemType Type, Action<EventSystemData> Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).RemoveCallback(Callback);
        }

        public static void RemoveCallback(Transform Master, EventSystemType Type, UnityAction Callback)
        {
            GetOrCreateHandler(Master.gameObject, Type).RemoveCallback(Callback);
        }

        public static void ClearCallback(Transform Master)
        {
            if (Master.GetComponent(typeof(EventSystemBaseHandler)) == null)
            {
                return;
            }

            for (var Index = 0; Index < EventTypeCount_; ++Index)
            {
                var Handler = Master.GetComponent(EventHandlerList_[(EventSystemType)Index]) as EventSystemBaseHandler;
                if (Handler != null)
                {
                    Handler.Dispose();
                    UnityEngine.Object.DestroyImmediate(Handler);
                }
            }
        }

        private static EventSystemBaseHandler GetOrCreateHandler(GameObject Master, EventSystemType Type)
        {
            var Handler = Master.GetComponent(EventHandlerList_[Type]);
            if (Handler == null)
            {
                Handler = Master.AddComponent(EventHandlerList_[Type]);
            }
            return Handler as EventSystemBaseHandler;
        }
    }
}