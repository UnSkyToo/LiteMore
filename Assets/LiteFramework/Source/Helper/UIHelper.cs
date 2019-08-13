using System;
using LiteFramework.Game.UI;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class UIHelper
    {
        public static Transform FindChild(Transform Parent, string ChildPath)
        {
            return Parent == null ? null : Parent.Find(ChildPath);
        }

        public static T FindComponent<T>(Transform Parent, string ChildPath) where T : Component
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                return Obj.GetComponent<T>();
            }
            return null;
        }

        public static Component FindComponent(Transform Parent, string ChildPath, Type CType)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                return Obj.GetComponent(CType);
            }
            return null;
        }

        public static Component FindComponent(Transform Parent, string ChildPath, string CType)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                return Obj.GetComponent(CType);
            }
            return null;
        }

        public static void AddEvent(Transform Obj, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
        }

        public static void AddEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
        }

        public static void RemoveEvent(Transform Obj, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
        }

        public static void RemoveEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
            }
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
            }
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
            }
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
            }
        }

        public static void ShowChild(Transform Parent, string ChildPath)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(true);
            }
        }

        public static void HideChild(Transform Parent, string ChildPath)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(false);
            }
        }

        public static void EnableTouched(Transform Target, bool Enabled)
        {
            if (Target == null)
            {
                return;
            }

            var Listener = Target.GetComponent<UIEventTriggerListener>();
            if (Listener != null)
            {
                Listener.enabled = Enabled;
            }
        }

        public static void EnableTouched(Transform Parent, string ChildPath, bool Enabled)
        {
            var Listener = FindComponent<UIEventTriggerListener>(Parent, ChildPath);
            if (Listener != null)
            {
                Listener.enabled = Enabled;
            }
        }

        public static void RemoveAllEvent(Transform Parent, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            UIEventTriggerListener.Remove(Parent);

            if (!Recursively)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);
                RemoveAllEvent(Child, Recursively);
            }
        }

        public static void RemoveAllChildren(Transform Parent)
        {
            if (Parent == null)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                UnityEngine.Object.Destroy(Parent.GetChild(Index)?.gameObject);
            }
        }

        public static void HideAllChildren(Transform Parent)
        {
            if (Parent == null)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                Parent.GetChild(Index)?.gameObject.SetActive(false);
            }
        }
    }
}