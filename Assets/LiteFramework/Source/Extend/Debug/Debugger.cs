using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class Debugger : MonoBehaviour
    {
        private bool IsMiniMode_ = true;

        public static float Scale { get; set; } = 1.0f;
        public static Rect MiniBounds { get; set; } = new Rect(10, 10, 100, 60);
        public static Rect NormalBounds { get; set; } = new Rect(10,10, 640, 480);

        private const string WindowTitle_ = "<b>Lite Debuger</b>";

        private readonly Rect TitleBarRect_ = new Rect(0, 0, Screen.width, 25);

        private int FpsCount_ = 60;
        private int CurrentFps_ = 0;
        private float FpsTime_ = 0.0f;

        private DebugGroupItem RootGroup_;

        void Awake()
        {
            RootGroup_ = new DebugGroupItem("Debugger");
            RootGroup_.Register("Log", new LogDebuggerItem());

            RootGroup_.Register("Info/System", new DebuggerInfo.SystemItem());
            RootGroup_.Register("Info/Environment", new DebuggerInfo.EnvironmentItem());
            RootGroup_.Register("Info/Screen", new DebuggerInfo.ScreenItem());
            RootGroup_.Register("Info/Graphics", new DebuggerInfo.GraphicsItem());
            RootGroup_.Register("Info/Input", new DebuggerInfo.InputItem());
            RootGroup_.Register("Info/Other", new DebuggerInfo.OtherItem());

            RootGroup_.Register("Setting", new SettingDebuggerItem());

            RootGroup_.Register("Profiler/Summary", new DebuggerProfiler.SummerItem());
            RootGroup_.Register("Profiler/Memory/Summary", new DebuggerProfiler.MemorySummaryItem());
            RootGroup_.Register("Profiler/Memory/All", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Object>());
            RootGroup_.Register("Profiler/Memory/Texture", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Texture>());
            RootGroup_.Register("Profiler/Memory/Mesh", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Mesh>());
            RootGroup_.Register("Profiler/Memory/Material", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Material>());
            RootGroup_.Register("Profiler/Memory/Shader", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Shader>());
            RootGroup_.Register("Profiler/Memory/AnimationClip", new DebuggerProfiler.MemoryInfoItem<UnityEngine.AnimationClip>());
            RootGroup_.Register("Profiler/Memory/AudioClip", new DebuggerProfiler.MemoryInfoItem<UnityEngine.AudioClip>());
            RootGroup_.Register("Profiler/Memory/Font", new DebuggerProfiler.MemoryInfoItem<UnityEngine.Font>());
            RootGroup_.Register("Profiler/Memory/TextAsset", new DebuggerProfiler.MemoryInfoItem<UnityEngine.TextAsset>());
            RootGroup_.Register("Profiler/Memory/ScriptableObject", new DebuggerProfiler.MemoryInfoItem<UnityEngine.ScriptableObject>());

            RootGroup_.Register("Profiler/Object Pool", new DebuggerProfiler.ObjectPoolItem());
        }

        void OnDestroy()
        {
            RootGroup_.Dispose();
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
                RootGroup_.Enter();
            }
        }

        private void DrawNormalWindow(int WindowID)
        {
            GUI.DragWindow(TitleBarRect_);

            if (GUILayout.Button($"<b>Fps:{FpsCount_}</b>", GUILayout.Width(100f), GUILayout.Height(40f)))
            {
                IsMiniMode_ = transform;
                RootGroup_.Exit();
            }
            RootGroup_.Draw();
        }
    }
}