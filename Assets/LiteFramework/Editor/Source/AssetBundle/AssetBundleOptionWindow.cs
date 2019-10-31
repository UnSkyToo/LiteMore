using LiteFramework.Helper;
using UnityEngine;
using UnityEditor;

namespace LiteFramework.Editor.AssetBundle
{
    public class AssetBundleOptionWindow : EditorWindow
    {
        public const int Width = 700;
        public const int Height = 400;

        private int CurrentSelectIndex_ = 0;

        private void OnEnable()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    CurrentSelectIndex_ = 3;
                    break;
                case BuildTarget.iOS:
                    CurrentSelectIndex_ = 2;
                    break;
                case BuildTarget.StandaloneOSX:
                    CurrentSelectIndex_ = 1;
                    break;
                default:
                    CurrentSelectIndex_ = 0;
                    break;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Platform");
            CurrentSelectIndex_ = GUILayout.SelectionGrid(CurrentSelectIndex_, new[] {"Windows", "Mac", "iOS", "Android"}, 4);

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

            if (GUILayout.Button("Delete Streaming Assets"))
            {
                AssetBundleBuilder.DeleteStreamingAssets();
            }

            if (GUILayout.Button("Generate FilesInfo File"))
            {
                AssetHelper.GenerateFilesInfoFile();
            }
        }

        private BuildTarget GetCurrentTarget()
        {
            switch (CurrentSelectIndex_)
            {
                case 0:
                    return BuildTarget.StandaloneWindows;
                case 1:
                    return BuildTarget.StandaloneOSX;
                case 2:
                    return BuildTarget.iOS;
                case 3:
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