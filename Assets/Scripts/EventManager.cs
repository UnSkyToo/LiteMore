using System;
using System.Collections.Generic;

namespace LiteMore
{
    public abstract class EventBase
    {
    }

    public static class EventManager
    {
        private abstract class EventListener
        {
            public abstract void Trigger(EventBase Msg);
        }

        private class EventListenerImpl<T> : EventListener where T : EventBase
        {
            public event Action<T> OnEvent = null;

            public override void Trigger(EventBase Msg)
            {
                OnEvent?.Invoke((T)Msg);
            }
        }

        private static readonly Dictionary<string, EventListener> EventList_ = new Dictionary<string, EventListener>();

        public static bool Startup()
        {
            EventList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
        }

        public static void Send<T>(T Event) where T : EventBase
        {
            var EventName = typeof(T).FullName;
            if (EventList_.ContainsKey(EventName))
            {
                ((EventListenerImpl<T>)EventList_[EventName]).Trigger(Event);
            }
        }

        public static void Send<T>() where T : EventBase, new()
        {
            var Event = new T();
            Send(Event);
        }

        public static void Register<T>(Action<T> Callback) where T : EventBase
        {
            var EventName = typeof(T).FullName;
            if (!EventList_.ContainsKey(EventName))
            {
                EventList_.Add(EventName, new EventListenerImpl<T>());
            }

            ((EventListenerImpl<T>)EventList_[EventName]).OnEvent += Callback;
        }

        public static void UnRegister<T>(Action<T> Callback) where T : EventBase
        {
            var EventName = typeof(T).FullName;
            if (EventList_.ContainsKey(EventName))
            {
                ((EventListenerImpl<T>)EventList_[EventName]).OnEvent -= Callback;
            }
        }
    }
}