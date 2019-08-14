using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class Debugger : MonoBehaviour
    {
        private bool IsMiniMode_ = true;

        public static float Scale { get; set; } = 1.0f;
        public static Rect MiniBounds { get; set; } = new Rect(10, 10, 100, 60);
        public static Rect NormalBounds { get; set; } = new Rect(10,10, 640, 480);

        private const string WindowTitle_ = "<b>Debuger</b>";

        private readonly Rect TitleBarRect_ = new Rect(0, 0, Screen.width, 20);

        private int FpsCount_ = 60;
        private int CurrentFps_ = 0;
        private float FpsTime_ = 0.0f;

        private Dictionary<DebugWindowType, BaseDebugWindow> WindowList_;
        private DebugWindowType CurrentWindow_;

        void Awake()
        {
            WindowList_ = new Dictionary<DebugWindowType, BaseDebugWindow>
            {
                {DebugWindowType.Log, new LogDebugWindow()},
                {DebugWindowType.Info, new InfoDebugWindow()},
                {DebugWindowType.Setting, new SettingDebugWindow()},
            };

            foreach (var Window in WindowList_)
            {
                Window.Value.Startup();
            }

            CurrentWindow_ = DebugWindowType.Log;
        }

        void OnDestroy()
        {
            foreach (var Window in WindowList_)
            {
                Window.Value.Shutdown();
            }
            WindowList_.Clear();
        }

        void Update()
        {
            FpsTime_ += Time.deltaTime;

            if (FpsTime_ >= 1.0f)
            {
                FpsTime_ -= 1.0f;
                FpsCount_ = CurrentFps_;
                CurrentFps_ = 0;
            }

            CurrentFps_++;

            if (!IsMiniMode_)
            {
                WindowList_[CurrentWindow_].Tick(Time.deltaTime);
            }
        }

        void OnGUI()
        {
            var CacheMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(Scale, Scale, 1));

            if (IsMiniMode_)
            {
                MiniBounds = GUILayout.Window(0, MiniBounds, DrawMiniWindow, WindowTitle_);
            }
            else
            {
                NormalBounds = GUILayout.Window(0, NormalBounds, DrawNormalWindow, WindowTitle_);
            }

            GUI.matrix = CacheMatrix;
        }

        public static void ResetLayout()
        {
            Scale = 1.0f;
            MiniBounds = new Rect(10, 10, 100, 60);
            NormalBounds = new Rect(10, 10, 640, 480);
        }

        private void DrawMiniWindow(int WindowID)
        {
            GUI.DragWindow(TitleBarRect_);
            GUILayout.Space(5);

            if (GUILayout.Button($"<b>Fps:{FpsCount_}</b>", GUILayout.Width(100f), GUILayout.Height(40f)))
            {
                IsMiniMode_ = false;
                CurrentWindow_ = DebugWindowType.Log;
            }
        }

        private void DrawNormalWindow(int WindowID)
        {
            GUI.DragWindow(TitleBarRect_);

            CurrentWindow_ = (DebugWindowType)GUILayout.Toolbar((int)CurrentWindow_, Enum.GetNames(typeof(DebugWindowType)), GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            if (CurrentWindow_ == DebugWindowType.Fps)
            {
                IsMiniMode_ = true;
                return;
            }

            WindowList_[CurrentWindow_].Draw();
        }
    }
}