using System;
using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Base;
using LiteFramework.Game.EventSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LiteFramework.Helper
{
    public static class UIHelper
    {
        private static readonly Material GrayMaterial_ = new Material(Shader.Find("Lite/GrayUI"));

        public static Vector2 ScreenPosToCanvasPos(RectTransform Parent, Vector2 ScreenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, ScreenPos, Camera.main, out var Pos);
            return Pos;
        }

        public static Vector2 ScreenPosToCanvasPos(Transform Parent, Vector2 ScreenPos)
        {
            var RectTrans = Parent.GetComponent<RectTransform>();
            if (RectTrans == null)
            {
                return Vector2.zero;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTrans, ScreenPos, Camera.main, out var Pos);
            return Pos;
        }

        public static Vector2 WorldPosToScreenPos(Vector3 WorldPos)
        {
            return RectTransformUtility.WorldToScreenPoint(Camera.main, WorldPos);
        }

        public static Vector2 WorldPosToCanvasPos(RectTransform Parent, Vector3 WorldPos)
        {
            return ScreenPosToCanvasPos(Parent, WorldPosToScreenPos(WorldPos));
        }

        public static Vector2 WorldPosToCanvasPos(Transform Parent, Vector3 WorldPos)
        {
            return ScreenPosToCanvasPos(Parent, WorldPosToScreenPos(WorldPos));
        }

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

        public static void AddEvent(Transform Obj, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.AddEvent(Obj, Callback, Type);
        }

        public static void AddEvent(GameEntity Entity, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.AddEvent(Entity, Callback, Type);
        }

        public static void AddEvent(Transform Obj, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (LiteConfigure.EnableButtonClick && Type == EventSystemType.Click)
            {
                var Btn = Obj?.GetComponent<Button>();
                if (Btn != null)
                {
                    Btn.onClick.AddListener(Callback);
                    return;
                }
            }
            
            EventHelper.AddEvent(Obj, Callback, Type);
        }

        public static void AddEvent(GameEntity Entity, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddClickEvent(Transform Obj, UnityAction Callback, AssetUri AudioUri)
        {
            void OnClick()
            {
                AudioManager.PlaySound(AudioUri);
                Callback?.Invoke();
            }

            AddEvent(Obj, OnClick);
        }

        public static void AddClickEvent(GameEntity Entity, UnityAction Callback, AssetUri AudioUri)
        {
            AddClickEvent(Entity?.GetTransform(), Callback, AudioUri);
        }

        public static void RemoveEvent(Transform Obj, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.RemoveEvent(Obj, Callback, Type);
        }

        public static void RemoveEvent(GameEntity Entity, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.RemoveEvent(Entity, Callback, Type);
        }

        public static void RemoveEvent(Transform Obj, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (LiteConfigure.EnableButtonClick && Type == EventSystemType.Click)
            {
                var Btn = Obj?.GetComponent<Button>();
                if (Btn != null)
                {
                    Btn.onClick.RemoveListener(Callback);
                    return;
                }
            }

            EventHelper.RemoveEvent(Obj, Callback, Type);
        }

        public static void RemoveEvent(GameEntity Entity, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.AddEventToChild(Parent, ChildPath, Callback, Type);
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.AddEventToChild(Entity, ChildPath, Callback, Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (LiteConfigure.EnableButtonClick && Type == EventSystemType.Click)
            {
                var Btn = Parent?.Find(ChildPath)?.GetComponent<Button>();
                if (Btn != null)
                {
                    Btn.onClick.AddListener(Callback);
                    return;
                }
            }

            EventHelper.AddEventToChild(Parent, ChildPath, Callback, Type);
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEventToChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void AddClickEventToChild(Transform Parent, string ChildPath, UnityAction Callback, AssetUri AudioUri)
        {
            void OnClick()
            {
                AudioManager.PlaySound(AudioUri);
                Callback?.Invoke();
            }

            AddEventToChild(Parent, ChildPath, OnClick);
        }

        public static void AddClickEventToChild(GameEntity Entity, string ChildPath, UnityAction Callback, AssetUri AudioUri)
        {
            AddClickEventToChild(Entity?.GetTransform(), ChildPath, Callback, AudioUri);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.RemoveEventFromChild(Parent, ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, Action<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            EventHelper.RemoveEventFromChild(Entity, ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (LiteConfigure.EnableButtonClick && Type == EventSystemType.Click)
            {
                var Btn = Parent?.Find(ChildPath)?.GetComponent<Button>();
                if (Btn != null)
                {
                    Btn.onClick.RemoveListener(Callback);
                    return;
                }
            }

            EventHelper.RemoveEventFromChild(Parent, ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEventFromChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void RemoveAllEvent(Transform Parent, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            if (LiteConfigure.EnableButtonClick)
            {
                var Btn = Parent.GetComponent<Button>();
                if (Btn != null)
                {
                    Btn.onClick.RemoveAllListeners();
                }
            }

            EventSystemListener.ClearCallback(Parent);

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

        public static void AddCanvasLayer(Transform Master, int Order)
        {
            if (Master == null)
            {
                return;
            }

            var Canvas = Master.GetOrAddComponent<Canvas>();
            Canvas.overrideSorting = true;
            Canvas.sortingOrder = Order;
            Master.GetOrAddComponent<GraphicRaycaster>();
        }

        public static void ReplaceSprite(Image Master, bool IsNativeSize, AssetUri Uri)
        {
            if (Master == null || Uri == null)
            {
                return;
            }

            AssetManager.CreateAssetAsync<Sprite>(Uri, (Spr) =>
            {
                if (Spr == null)
                {
                    LLogger.LWarning($"can't load {Uri}");
                    return;
                }

                Master.sprite = Spr;

                if (IsNativeSize)
                {
                    Master.SetNativeSize();
                }
            });
        }

        public static void ChangeColor(Transform Parent, Color NewColor, bool Recursively)
        {
            var Graphics = Parent?.GetComponent<Graphic>();
            if (Graphics == null)
            {
                return;
            }

            Graphics.color = NewColor;

            if (!Recursively)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);
                ChangeColor(Child, NewColor, Recursively);
            }
        }
    }
}