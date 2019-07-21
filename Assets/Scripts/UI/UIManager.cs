using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.UI
{
    public static class UIManager
    {
        private static Transform CanvasNormalTransform_ = null;
        private static Transform CanvasNormalTransform
        {
            get
            {
                if (CanvasNormalTransform_ == null)
                {
                    CanvasNormalTransform_ = GameObject.Find("Canvas-Normal").transform;
                }

                return CanvasNormalTransform_;
            }
        }

        private static readonly Dictionary<uint, UIBase> UIList_ = new Dictionary<uint, UIBase>();
        private static readonly Dictionary<string, Transform> CacheList_ = new Dictionary<string, Transform>();
        private static readonly List<UIBase> OpenList_ = new List<UIBase>();
        private static readonly List<UIBase> CloseList_ = new List<UIBase>();

        public static bool Startup()
        {
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
                UnityEngine.Object.Destroy(Child.gameObject);
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
                        UnityEngine.Object.Destroy(UI.UITransform.gameObject);
                    }
                }

                CloseList_.Clear();
            }
        }

        public static T OpenUI<T>(params object[] Params) where T : UIBase, new()
        {
            var ScriptType = typeof(T);
            if (!UIConfigure.UIList.ContainsKey(ScriptType))
            {
                Debug.LogWarning($"Can't find UIPath : {ScriptType.Name}");
                return null;
            }

            var Desc = UIConfigure.UIList[ScriptType];

            if (Desc.OpenMore)
            {
                return CreateUI<T>(Desc, Params);
            }

            return FindUI<T>() ?? CreateUI<T>(Desc, Params);
        }

        public static T OpenUI<T>(UIDescriptor Desc, params object[] Params) where T : UIBase, new()
        {
            return CreateUI<T>(Desc, Params);
        }

        public static T OpenUI<T>(T Script, UIDescriptor Desc, params object[] Params) where T : UIBase
        {
            return CreateUI<T>(Script, Desc, Params);
        }

        public static void CloseUI<T>() where T : UIBase
        {
            var UI = FindUI<T>();
            if (UI != null)
            {
                CloseUI(UI);
            }
        }

        public static void CloseUI(UIBase UI)
        {
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
                UnityEngine.Object.Destroy(Cache.Value.gameObject);
            }

            CacheList_.Clear();
        }

        public static T FindUI<T>() where T : UIBase
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

        public static bool IsClosed<T>() where T : UIBase
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
            Transform UIObj = null;
            if (CacheList_.ContainsKey(Desc.PrefabName) && !Desc.OpenMore)
            {
                UIObj = CacheList_[Desc.PrefabName];
                CacheList_.Remove(Desc.PrefabName);
            }

            if (UIObj == null)
            {
                var UIPath = $"UI/{Desc.PrefabName}";
                var Model = Resources.Load<GameObject>(UIPath);
                if (Model == null)
                {
                    Debug.LogWarning($"Can't Create UI : {UIPath}");
                    return null;
                }

                return UnityEngine.Object.Instantiate(Model).transform;
            }

            return UIObj;
        }

        public static T CreateUI<T>(UIDescriptor Desc, params object[] Params) where T : UIBase, new()
        {
            var Obj = GetOrCreateGameObject(Desc);
            return CreateUI<T>(Obj, Desc, Params);
        }

        public static T CreateUI<T>(T Script, UIDescriptor Desc, params object[] Params) where T : UIBase
        {
            var Obj = GetOrCreateGameObject(Desc);
            return CreateUI<T>(Obj, Script, Desc, Params);
        }

        public static T CreateUI<T>(Transform Obj, UIDescriptor Desc, params object[] Params) where T : UIBase, new()
        {
            var Script = new T();
            return CreateUI<T>(Obj, Script, Desc, Params);
        }

        public static T CreateUI<T>(Transform Obj, T Script, UIDescriptor Desc, params object[] Params) where T : UIBase
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
            var NewList = new List<UIBase>(UIList_.Values);
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