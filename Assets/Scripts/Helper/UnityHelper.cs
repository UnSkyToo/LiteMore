using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LiteMore.Helper
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
#if UNITY_EDITOR
            /*var LogEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var ClearMethod = LogEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            ClearMethod.Invoke(null, null);*/

            Debug.ClearDeveloperConsole();
#endif
        }

        public static void SetResolution(int Width, int Height)
        {
#if UNITY_EDITOR
            Screen.SetResolution(Width, Height, false);

            var GameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
            var GetMainGameViewFunc = GameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var GameView = GetMainGameViewFunc.Invoke(null, null) as UnityEditor.EditorWindow;
            var GameViewSizeProp = GameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ViewSize = GameViewSizeProp.GetValue(GameView, new object[0] { });
            var ViewSizeType = ViewSize.GetType();

            ViewSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(ViewSize, Width, new object[0] { });
            ViewSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(ViewSize, Height, new object[0] { });

            var UpdateZoomAreaAndParentFunc = GameViewType.GetMethod("UpdateZoomAreaAndParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            UpdateZoomAreaAndParentFunc.Invoke(GameView, null);
#endif
        }

        private static readonly RectTransform RootCanvas_ = Configure.CanvasRoot.GetComponent<RectTransform>();
        public static Vector2 ScreenPosToCanvasPos(Vector2 ScreenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas_, ScreenPos, Camera.main, out var Pos);
            return Pos;
        }

        public static Color RandColor()
        {
            return new Color(Random.value, Random.value, Random.value);
        }
    }
}