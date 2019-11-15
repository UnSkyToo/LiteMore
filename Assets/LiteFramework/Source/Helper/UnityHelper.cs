using UnityEngine;

namespace LiteFramework.Helper
{
    public static class UnityHelper
    {
#if UNITY_EDITOR
        public static void SetResolution(int Width, int Height)
        {
            Screen.SetResolution(Width, Height, false);

            var GameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            var GetMainGameViewFunc = GameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var GameView = GetMainGameViewFunc.Invoke(null, null) as UnityEditor.EditorWindow;
            var GameViewSizeProp = GameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ViewSize = GameViewSizeProp.GetValue(GameView, new object[0] { });
            var ViewSizeType = ViewSize.GetType();

            ViewSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(ViewSize, Width, new object[0] { });
            ViewSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(ViewSize, Height, new object[0] { });

            var UpdateZoomAreaAndParentFunc = GameViewType.GetMethod("UpdateZoomAreaAndParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            UpdateZoomAreaAndParentFunc.Invoke(GameView, null);
        }

        public static void ShowEditorNotification(string Msg)
        {
            var Func = typeof(UnityEditor.SceneView).GetMethod("ShowNotification", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Func?.Invoke(null, new object[] { Msg });
        }
#endif

        public static void ChangeLayer(GameObject Parent, int Layer)
        {
            Parent.layer = Layer;

            var Children = Parent.GetComponentsInChildren<Transform>();
            foreach (var Child in Children)
            {
                Child.gameObject.layer = Layer;
            }
        }

        public static void ChangeSortingOrder(GameObject Parent, int Order)
        {
            var Render = Parent.GetComponent<Renderer>();
            if (Render != null)
            {
                Render.sortingOrder = Order;
            }

            var Children = Parent.GetComponentsInChildren<Renderer>();
            foreach (var Child in Children)
            {
                Child.sortingOrder = Order;
            }
        }

        public static void AddSortingOrder(GameObject Parent, int Order)
        {
            var Render = Parent.GetComponent<Renderer>();
            if (Render != null)
            {
                Render.sortingOrder += Order;
            }

            var Canvas = Parent.GetComponent<Canvas>();
            if (Canvas != null)
            {
                Canvas.sortingOrder += Order;
            }

            var ChildrenR = Parent.GetComponentsInChildren<Renderer>();
            foreach (var Child in ChildrenR)
            {
                Child.sortingOrder += Order;
            }

            var ChildrenC = Parent.GetComponentsInChildren<Canvas>();
            foreach (var Child in ChildrenC)
            {
                Child.sortingOrder += Order;
            }
        }

        public static int GetSortingOrderUpper(GameObject Parent)
        {
            var Canvas = GetComponentUpper<Canvas>(Parent?.transform);

            if (Canvas != null)
            {
                return Canvas.sortingOrder;
            }

            return 0;
        }

        public static T GetComponentUpper<T>(Transform Parent) where T : Component
        {
            while (Parent != null)
            {
                var Comp = Parent.GetComponent<T>();
                if (Comp != null)
                {
                    return Comp;
                }

                Parent = Parent.parent;
            }

            return null;
        }

        public static T GetOrAddComponent<T>(this GameObject Master) where T : Component
        {
            return GetOrAddComponentSafe<T>(Master);
        }

        public static T GetOrAddComponent<T>(this Transform Master) where T : Component
        {
            return GetOrAddComponentSafe<T>(Master?.transform);
        }

        public static T GetOrAddComponentSafe<T>(GameObject Master) where T : Component
        {
            if (Master == null)
            {
                return null;
            }

            var ConT = Master.GetComponent<T>();
            if (ConT == null)
            {
                ConT = Master.AddComponent<T>();
            }

            return ConT;
        }

        public static T GetOrAddComponentSafe<T>(Transform Master) where T : Component
        {
            return GetOrAddComponentSafe<T>(Master?.gameObject);
        }
    }
}