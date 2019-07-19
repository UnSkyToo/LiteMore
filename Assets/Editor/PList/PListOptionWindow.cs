using UnityEngine;
using UnityEditor;

namespace Lite.Editor.PList
{
    public class PListOptionWindow : EditorWindow
    {
        public const int Width = 700;
        public const int Height = 400;

        private static UnityEngine.Object SelectObject_;

        private void OnGUI()
        {
            GUILayout.Label("Import PList Data");
            SelectObject_ = EditorGUILayout.ObjectField(SelectObject_, typeof(UnityEngine.Object), false);
            if (GUILayout.Button("Import"))
            {
                PListImporter.ImportPList(SelectObject_);
            }
        }

        public static void ShowWindow()
        {
            var Window = EditorWindow.GetWindow<PListOptionWindow>(true, "Plist Import Window", true);
            Window.minSize = new Vector2(Width, Height);
            Window.maxSize = new Vector2(Width, Height);
            Window.position = new Rect((Screen.currentResolution.width - Width) / 2,
                (Screen.currentResolution.height - Height) / 2, Width, Height);
        }
    }
}