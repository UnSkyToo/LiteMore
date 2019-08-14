using UnityEngine;
using UnityEditor;

namespace LiteFramework.Editor.AssetBundle
{
    public class AssetBundleOptionWindow : EditorWindow
    {
        public const int Width = 700;
        public const int Height = 400;

        private int CurrentSelectIndex_ = 0;

        private void OnGUI()
        {
            GUILayout.Label("Platform");
            CurrentSelectIndex_ = GUILayout.SelectionGrid(CurrentSelectIndex_, new[] {"Windows", "iOS", "Android"}, 3);

            if (GUILayout.Button("Start Build"))
            {
                AssetBundleBuilder.BuildAsset(GetCurrentTarget(), GetCurrentOptions(), true);
            }

            if (GUILayout.Button("Start Build With No Collect"))
            {
                AssetBundleBuilder.BuildAsset(GetCurrentTarget(), GetCurrentOptions(), false);
            }

            if (GUILayout.Button("Clean All AssetBundle"))
            {
                AssetBundleBuilder.CleanAllBundle();
            }
        }

        private BuildTarget GetCurrentTarget()
        {
            switch (CurrentSelectIndex_)
            {
                case 0:
                    return BuildTarget.StandaloneWindows;
                case 1:
                    return BuildTarget.iOS;
                case 2:
                    return BuildTarget.Android;
                default:
                    break;
            }

            return BuildTarget.StandaloneWindows;
        }

        private BuildAssetBundleOptions GetCurrentOptions()
        {
            var Options = BuildAssetBundleOptions.None;
            //Options |= BuildAssetBundleOptions.ChunkBasedCompression; // LZ4 Compression, fast
            return Options;
        }

        public static void ShowWindow()
        {
            var Window = EditorWindow.GetWindow<AssetBundleOptionWindow>(true, "AssetBundle Builder Option", true);
            Window.minSize = new Vector2(Width, Height);
            Window.maxSize = new Vector2(Width, Height);
            Window.position = new Rect((Screen.currentResolution.width - Width) / 2,
                (Screen.currentResolution.height - Height) / 2, Width, Height);
        }
    }
}