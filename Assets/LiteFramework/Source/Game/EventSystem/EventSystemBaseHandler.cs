using System;
using UnityEngine;
using UnityEngine.Events;

namespace LiteFramework.Game.EventSystem
{
    public abstract class EventSystemBaseHandler : MonoBehaviour, IDisposable
    {
        protected Action<EventSystemData> EventCallback_;
        protected UnityAction EventCallbackEx_;

        public void Dispose()
        {
            EventCallback_ = null;
            EventCallbackEx_ = null;
        }

        public void AddCallback(Action<EventSystemData> Callback)
        {
            EventCallback_ += Callback;
        }

        public void AddCallback(UnityAction Callback)
        {
            EventCallbackEx_ += Callback;
        }

        public void RemoveCallback(Action<EventSystemData> Callback)
        {
            if (Callback == null)
            {
                EventCallback_ = null;
            }
            else
            {
                EventCallback_ -= Callback;
            }
        }

        public void RemoveCallback(UnityAction Callback)
        {
            if (Callback == null)
            {
                EventCallbackEx_ = null;
            }
            else
            {
                EventCallbackEx_ -= Callback;
            }
        }
    }
}