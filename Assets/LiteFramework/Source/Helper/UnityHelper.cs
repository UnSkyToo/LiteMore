using UnityEngine;

namespace LiteFramework.Helper
{
    public static class UnityHelper
    {
        public static string GetDeviceID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static string GetPlatform()
        {
#if UNITY_IPHONE
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "Windows";
#endif
        }

        public static void ClearLog()
        {
/*#if UNITY_EDITOR
            var LogEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var ClearMethod = LogEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            ClearMethod.Invoke(null, null);
#endif*/
            Debug.ClearDeveloperConsole();
        }

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

        public static Color RandColor()
        {
            return new Color(Random.value, Random.value, Random.value);
        }

        public static Vector2 RandVec2(float Radius)
        {
            return new Vector2(Random.Range(-Radius, Radius), Random.Range(-Radius, Radius));
        }

        public static Vector2 RandCircle(float Radius)
        {
            var Dist = Random.Range(1, Radius);
            var Angle = Random.Range(0, 360);
            var Rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
            var Dir = Rotation * Vector3.up;
            return Dir.normalized * Dist;
        }
    }
}