using System;
using System.Collections.Generic;
using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Game.UI
{
    public static class UIManager
    {
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

        private static RectTransform CanvasNormalRectTransform_ = null;
        public static RectTransform CanvasNormalRectTransform
        {
            get
            {
                if (CanvasNormalRectTransform_ == null)
                {
                    CanvasNormalRectTransform_ = CanvasNormalTransform.GetComponent<RectTransform>();
                }

                return CanvasNormalRectTransform_;
            }
        }

        private static readonly Dictionary<uint, BaseUI> UIList_ = new Dictionary<uint, BaseUI>();
        private static readonly Dictionary<string, Transform> CacheList_ = new Dictionary<string, Transform>();
        private static readonly List<BaseUI> OpenList_ = new List<BaseUI>();
        private static readonly List<BaseUI> CloseList_ = new List<BaseUI>();

        public static bool Startup()
        {
            CanvasNormalRectTransform_ = null;
            CanvasNormalRectTransform_ = null;

            UIList_.Clear();
            CacheList_.Clear();
            OpenList_.Clear();
            CloseList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            CloseAllUI();

            var ChildCount = CanvasNormalTransform.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = CanvasNormalTransform.GetChild(Index);
                AssetManager.DeleteAsset(Child.gameObject);
            }

            UIList_.Clear();
            CacheList_.Clear();
            OpenList_.Clear();
            CloseList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            if (OpenList_.Count > 0)
            {
                foreach (var UI in OpenList_)
                {
                    UIList_.Add(UI.ID, UI);
                }

                OpenList_.Clear();
                ResortUIList();
            }

            if (UIList_.Count > 0)
            {
                foreach (var UI in UIList_)
                {
                    UI.Value.Tick(DeltaTime);
                }
            }

            if (CloseList_.Count > 0)
            {
                foreach (var UI in CloseList_)
                {
                    UIList_.Remove(UI.ID);

                    if (UI.Cached && !CacheList_.ContainsKey(UI.Path))
                    {
                        CacheList_.Add(UI.Path, UI.UITransform);
                    }
                    else
                    {
                        AssetManager.DeleteAsset(UI.UIRectTransform.gameObject);
                    }
                }

                CloseList_.Clear();
            }
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
            CloseList_.Add(UI);
        }

        public static void CloseAllUI()
        {
            if (UIList_.Count > 0)
            {
                foreach (var UI in UIList_)
                {
                    CloseUI(UI.Value);
                }

                UIList_.Clear();
            }

            if (OpenList_.Count > 0)
            {
                foreach (var UI in OpenList_)
                {
                    CloseUI(UI);
                }

                OpenList_.Clear();
            }
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

            foreach (var Data in UIList_)
            {
                if (Data.Value.Name == ScriptType.Name)
                {
                    return Data.Value as T;
                }
            }

            foreach (var Data in OpenList_)
            {
                if (Data.Name == ScriptType.Name)
                {
                    return Data as T;
                }
            }

            return null;
        }

        public static List<T> FindAllUI<T>() where T : BaseUI
        {
            var ScriptType = typeof(T);
            var Result = new List<T>();

            foreach (var Data in UIList_)
            {
                if (Data.Value.Name == ScriptType.Name)
                {
                    Result.Add(Data.Value as T);
                }
            }

            foreach (var Data in OpenList_)
            {
                if (Data.Name == ScriptType.Name)
                {
                    Result.Add(Data as T);
                }
            }

            return Result;
        }

        public static bool IsOpened<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            if (UI == null)
            {
                return false;
            }

            return true;
        }

        public static bool IsClosed<T>() where T : BaseUI
        {
            var UI = FindUI<T>();
            if (UI == null)
            {
                return true;
            }

            return false;
        }

        private static Transform GetOrCreateGameObject(UIDescriptor Desc)
        {
            if (CacheList_.ContainsKey(Desc.PrefabName) && !Desc.OpenMore)
            {
                var UIObj = CacheList_[Desc.PrefabName];
                CacheList_.Remove(Desc.PrefabName);
                return UIObj;
            }

            var UIPath = $"{Desc.PrefabName}.prefab";
            var Obj = AssetManager.CreatePrefabSync(UIPath);
            if (Obj == null)
            {
                LLogger.LError($"Can't Create UI : {Desc.PrefabName}");
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
            Obj.name = $"{Desc.PrefabName}<{Script.ID}>";
            Obj.SetParent(CanvasNormalTransform, false);

            Script.Path = Desc.PrefabName;
            Script.Cached = Desc.Cached;
            Script.UITransform = Obj;
            Script.UIRectTransform = Obj.GetComponent<RectTransform>();
            Script.UIRectTransform.SetSiblingIndex(Script.SortOrder + UIList_.Count + CacheList_.Count);

            OpenList_.Add(Script);
            Script.Open(Params);
            return Script;
        }

        private static void ResortUIList()
        {
            var CountInfo = GetUICountInfo();
            RebuildUIList();

            var BottomIndex = CacheList_.Count;
            var NormalIndex = BottomIndex + CountInfo[UIDepthMode.Bottom];
            var TopIndex = NormalIndex + CountInfo[UIDepthMode.Normal];

            foreach (var UI in UIList_)
            {
                var NewIndex = UI.Value.UIRectTransform.GetSiblingIndex();
                switch (UI.Value.DepthMode)
                {
                    case UIDepthMode.Bottom:
                        NewIndex += BottomIndex++;
                        break;
                    case UIDepthMode.Normal:
                        NewIndex += NormalIndex++;
                        break;
                    case UIDepthMode.Top:
                        NewIndex += TopIndex++;
                        break;
                    default:
                        break;
                }

                UI.Value.UIRectTransform.SetSiblingIndex(NewIndex);
            }
        }

        private static Dictionary<UIDepthMode, int> GetUICountInfo()
        {
            var Result = new Dictionary<UIDepthMode, int>
            {
                {UIDepthMode.Bottom, 0},
                {UIDepthMode.Normal, 0},
                {UIDepthMode.Top, 0}
            };

            foreach (var UI in UIList_)
            {
                Result[UI.Value.DepthMode]++;
            }

            return Result;
        }

        private static void RebuildUIList()
        {
            var NewList = new List<BaseUI>(UIList_.Values);
            NewList.Sort((X, Y) =>
            {
                if (X.UIRectTransform.GetSiblingIndex() + X.SortOrder < Y.UIRectTransform.GetSiblingIndex() + Y.SortOrder)
                {
                    return -1;
                }

                if (X.UIRectTransform.GetSiblingIndex() + X.SortOrder > Y.UIRectTransform.GetSiblingIndex() + Y.SortOrder)
                {
                    return 1;
                }

                return 0;
            });

            UIList_.Clear();
            foreach (var UI in NewList)
            {
                UIList_.Add(UI.ID, UI);
            }
        }
    }
}