using System;
using UnityEngine;

namespace LiteFramework.Game.UI.Event
{
    public abstract class UIEventBaseHandler : MonoBehaviour
    {
        protected Action<GameObject, Vector2> EventCallback_;
        protected Action EventCallbackEx_;

        public void AddCallback(Action<GameObject, Vector2> Callback)
        {
            EventCallback_ += Callback;
        }

        public void AddCallback(Action Callback)
        {
            EventCallbackEx_ += Callback;
        }

        public void RemoveCallback(Action<GameObject, Vector2> Callback)
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

        public void RemoveCallback(Action Callback)
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