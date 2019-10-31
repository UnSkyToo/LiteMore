using System.Collections.Generic;
using LiteFramework.Core.Base;
using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Game.UI
{
    public static class UIManager
    {
        public const int OrderStep = 100;

        private static Transform CanvasBottomTransform_ = null;
        public static Transform CanvasBottomTransform
        {
            get
            {
                if (CanvasBottomTransform_ == null)
                {
                    CanvasBottomTransform_ = GameObject.Find(LiteConfigure.CanvasBottomName).transform;
                }

                return CanvasBottomTransform_;
            }
        }

        private static Transform CanvasNormalTransform_ = null;
        public static Transform CanvasNormalTransform
        {
            get
            {
                if (CanvasNormalTransform_ == null)
                {
                    CanvasNormalTransform_ = GameObject.Find(LiteConfigure.CanvasNormalName).transform;
                }

                return CanvasNormalTransform_;
            }
        }

        private static Transform CanvasTopTransform_ = null;
        public static Transform CanvasTopTransform
        {
            get
            {
                if (CanvasTopTransform_ == null)
                {
                    CanvasTopTransform_ = GameObject.Find(LiteConfigure.CanvasTopName).transform;
                }

                return CanvasTopTransform_;
            }
        }

        private static readonly ListEx<BaseUI> UIList_ = new ListEx<BaseUI>();
        private static readonly Dictionary<UIDepthMode, int> UIDepthCount_ = new Dictionary<UIDepthMode, int>();
        private static readonly Dictionary<string, Transform> CacheList_ = new Dictionary<string, Transform>();
        private static float DeltaTime_;

        public static bool Startup()
        {
            CanvasNormalTransform_ = null;

            UIList_.Clear();
            CacheList_.Clear();

            UIDepthCount_.Clear();
            UIDepthCount_.Add(UIDepthMode.Bottom, 0);
            UIDepthCount_.Add(UIDepthMode.Normal, 0);
            UIDepthCount_.Add(UIDepthMode.Top, 0);

            return true;
        }

        public static void Shutdown()
        {
            CloseAllUI();

            var ChildCount = CanvasBottomTransform.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = CanvasBottomTransform.GetChild(Index);
                AssetManager.DeleteAsset(Child.gameObject);
            }

            ChildCount = CanvasNormalTransform.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = CanvasNormalTransform.GetChild(Index);
                AssetManager.DeleteAsset(Child.gameObject);
            }

            ChildCount = CanvasTopTransform.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = CanvasTopTransform.GetChild(Index);
                AssetManager.DeleteAsset(Child.gameObject);
            }

            UIList_.Clear();
            UIDepthCount_.Clear();
            CacheList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            DeltaTime_ = DeltaTime;
            UIList_.Foreach((Entity) =>
            {
                Entity.Tick(DeltaTime_);
            });
        }

        public static T OpenUI<T>(params object[] Params) where T : BaseUI, new()
        {
            var ScriptType = typeof(T);
            if (!LiteConfigure.UIDescList.ContainsKey(ScriptType))
            {
                LLogger.LError($"Can't find UI Desc : {ScriptType.Name}");
                return null;
            }

            return OpenUI<T>(LiteConfigure.UIDescList[ScriptType], Params);
        }

        public static T OpenUI<T>(UIDescriptor Desc, params object[] Params) where T : BaseUI, new()
        {
            return OpenUI(new T(), Desc, Params);
        }

        public static T OpenUI<T>(T Script, UIDescriptor Desc, params object[] Params) where T : BaseUI
        {
            if (Desc.OpenMore)
            {
                return CreateUI<T>(Script, Desc, Params);
            }

            if (IsOpened<T>())
            {
                return FindUI<T>();
            }

            return CreateUI<T>(Script, Desc, Params);
        }

        public static void ShowUI<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            UI?.Show();
        }

        public static void HideUI<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            UI?.Hide();
        }

        public static void CloseUI<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            if (UI != null)
            {
                CloseUI(UI);
            }
        }

        public static void CloseUI(BaseUI UI)
        {
            if (UI == null || UI.IsClosed)
            {
                return;
            }

            UI.Close();
            UIList_.Remove(UI);
            UIDepthCount_[UI.DepthMode]--;

            if (UI.Cached && !CacheList_.ContainsKey(UI.Path))
            {
                CacheList_.Add(UI.Path, UI.UITransform);
            }
            else
            {
                AssetManager.DeleteAsset(UI.UIRectTransform.gameObject);
            }
        }

        public static void CloseAllUI()
        {
            UIList_.Foreach(CloseUI);
            UIList_.Clear();
        }

        public static void DeleteUnusedUI()
        {
            foreach (var Cache in CacheList_)
            {
                AssetManager.DeleteAsset(Cache.Value.gameObject);
            }

            CacheList_.Clear();
        }

        public static T FindUI<T>() where T : BaseUI
        {
            var ScriptType = typeof(T);
            return UIList_.Where((Entity) => Entity.Name == ScriptType.Name) as T;
        }

        public static List<T> FindAllUI<T>() where T : BaseUI
        {
            var ScriptType = typeof(T);
            return UIList_.All((Entity) => Entity.Name == ScriptType.Name) as List<T>;
        }

        public static bool IsOpened<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            if (UI == null || UI.IsClosed)
            {
                return false;
            }

            return true;
        }

        public static bool IsClosed<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            if (UI == null || UI.IsClosed)
            {
                return true;
            }

            return false;
        }

        private static Transform GetOrCreateGameObject(UIDescriptor Desc)
        {
            if (CacheList_.ContainsKey(Desc.Uri) && !Desc.OpenMore)
            {
                var UIObj = CacheList_[Desc.Uri];
                CacheList_.Remove(Desc.Uri);
                return UIObj;
            }

            var Obj = AssetManager.CreatePrefabSync(Desc.Uri);
            if (Obj == null)
            {
                LLogger.LError($"Can't Create UI : {Desc.Uri}");
                return null;
            }

            return Obj.transform;
        }

        private static T CreateUI<T>(T Script, UIDescriptor Desc, params object[] Params) where T : BaseUI
        {
            var Obj = GetOrCreateGameObject(Desc);
            if (Obj == null)
            {
                return null;
            }
            return CreateUI<T>(Obj, Script, Desc, Params);
        }

        public static T CreateUI<T>(Transform Obj, T Script, UIDescriptor Desc, params object[] Params) where T : BaseUI
        {
            Obj.name = $"{Desc.Uri.AssetName}<{Script.ID}>";
            Obj.localRotation = Quaternion.identity;

            switch (Script.DepthMode)
            {
                case UIDepthMode.Bottom:
                    Obj.SetParent(CanvasBottomTransform, false);
                    break;
                case UIDepthMode.Normal:
                    Obj.SetParent(CanvasNormalTransform, false);
                    break;
                case UIDepthMode.Top:
                    Obj.SetParent(CanvasTopTransform, false);
                    break;
                default:
                    break;
            }

            Script.Path = Desc.Uri;
            Script.Cached = Desc.Cached;
            Script.UITransform = Obj;
            Script.UIRectTransform = Obj.GetComponent<RectTransform>();
            Script.UIRectTransform.SetSiblingIndex(Script.DepthIndex + UIDepthCount_[Script.DepthMode]);
            Script.UICanvas.sortingOrder = (int)Script.DepthMode + (Script.DepthIndex + UIDepthCount_[Script.DepthMode]) * OrderStep;

            UIDepthCount_[Script.DepthMode]++;
            UIList_.Add(Script);
            Script.Open(Params);
            return Script;
        }
    }
}