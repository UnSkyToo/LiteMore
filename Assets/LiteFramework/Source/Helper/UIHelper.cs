using System;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Base;
using LiteFramework.Game.UI;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class UIHelper
    {
        private static readonly Material GrayMaterial_ = new Material(Shader.Find("Lite/GrayUI"));

        public static Transform FindChild(Transform Parent, string ChildPath)
        {
            return Parent == null ? null : Parent.Find(ChildPath);
        }

        public static T GetComponent<T>(Transform Parent, string ChildPath) where T : Component
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                return Obj.GetComponent<T>();
            }
            return null;
        }

        public static Component GetComponent(Transform Parent, string ChildPath, Type CType)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                return Obj.GetComponent(CType);
            }
            return null;
        }

        public static Component GetComponent(Transform Parent, string ChildPath, string CType)
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

            UIEventListener.AddCallback(Obj, Type, Callback);
        }

        public static void AddEvent(GameEntity Entity, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            AddEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventListener.AddCallback(Obj, Type, Callback);
        }

        public static void AddEvent(GameEntity Entity, Action Callback, UIEventType Type = UIEventType.Click)
        {
            AddEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddClickEvent(Transform Obj, Action Callback, AssetUri AudioUri)
        {
            void OnClick()
            {
                AudioManager.PlaySound(AudioUri);
                Callback?.Invoke();
            }

            AddEvent(Obj, OnClick);
        }

        public static void AddClickEvent(GameEntity Entity, Action Callback, AssetUri AudioUri)
        {
            AddClickEvent(Entity?.GetTransform(), Callback, AudioUri);
        }

        public static void RemoveEvent(Transform Obj, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventListener.RemoveCallback(Obj, Type, Callback);
        }

        public static void RemoveEvent(GameEntity Entity, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void RemoveEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            UIEventListener.RemoveCallback(Obj, Type, Callback);
        }

        public static void RemoveEvent(GameEntity Entity, Action Callback, UIEventType Type = UIEventType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventListener.AddCallback(Obj, Type, Callback);
            }
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            AddEventToChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventListener.AddCallback(Obj, Type, Callback);
            }
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            AddEventToChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void AddClickEventToChild(Transform Parent, string ChildPath, Action Callback, AssetUri AudioUri)
        {
            void OnClick()
            {
                AudioManager.PlaySound(AudioUri);
                Callback?.Invoke();
            }

            AddEventToChild(Parent, ChildPath, OnClick);
        }

        public static void AddClickEventToChild(GameEntity Entity, string ChildPath, Action Callback, AssetUri AudioUri)
        {
            AddClickEventToChild(Entity?.GetTransform(), ChildPath, Callback, AudioUri);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventListener.RemoveCallback(Obj, Type, Callback);
            }
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, Action<GameObject, Vector2> Callback, UIEventType Type = UIEventType.Click)
        {
            RemoveEventFromChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventListener.RemoveCallback(Obj, Type, Callback);
            }
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            RemoveEventFromChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void SetActive(Transform Parent, string ChildPath, bool Value)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(Value);
            }
        }

        public static void SetActive(GameEntity Entity, string ChildPath, bool Value)
        {
            SetActive(Entity?.GetTransform(), ChildPath, Value);
        }

        public static void EnableTouched(Transform Target, bool Value)
        {
            if (Target == null)
            {
                return;
            }

            var Listener = Target.GetComponent<UnityEngine.UI.Graphic>();
            if (Listener != null)
            {
                Listener.raycastTarget = Value;
            }
        }

        public static void EnableTouched(Transform Parent, string ChildPath, bool Value)
        {
            var Listener = GetComponent<UnityEngine.UI.Graphic>(Parent, ChildPath);
            if (Listener != null)
            {
                Listener.raycastTarget = Value;
            }
        }

        public static void RemoveAllEvent(Transform Parent, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            UIEventListener.ClearCallback(Parent);

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

        public static void RemoveAllEvent(GameEntity Entity, bool Recursively)
        {
            RemoveAllEvent(Entity?.GetTransform(), Recursively);
        }

        public static void RemoveAllChildren(Transform Parent, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);

                if (Recursively)
                {
                    RemoveAllChildren(Child, Recursively);
                }

                AssetManager.DeleteAsset(Child?.gameObject);
            }
        }

        public static void RemoveAllChildren(GameEntity Entity, bool Recursively)
        {
            RemoveAllChildren(Entity?.GetTransform(), Recursively);
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

        public static void EnableGray(UnityEngine.UI.Graphic Master, bool Enabled)
        {
            if (Master == null)
            {
                return;
            }

            Master.material = Enabled ? GrayMaterial_ : null;
        }

        public static void EnableGray(Transform Parent, bool Enabled, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            var UIGraphics = Parent.GetComponent<UnityEngine.UI.Graphic>();
            EnableGray(UIGraphics, Enabled);

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);

                if (Recursively)
                {
                    EnableGray(Child, Enabled, Recursively);
                }
            }
        }
    }
}