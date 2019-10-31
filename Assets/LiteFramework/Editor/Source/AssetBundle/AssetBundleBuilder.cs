using System.IO;
using System.Collections.Generic;
using LiteFramework.Core.Log;
using LiteFramework.Helper;
using UnityEngine;
using UnityEditor;

namespace LiteFramework.Editor.AssetBundle
{
    public class AssetBundleBuilder : MonoBehaviour
    {
        private static readonly List<string> LuaFileList_ = new List<string>();

        // % Ctrl & Alt # Shift
        [MenuItem("Lite/AssetBundle/Builder %#L", false, 1)]
        private static void BuildAssets()
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                EditorUtility.DisplayDialog("Lite", "building is running", "Ok");
                return;
            }

            AssetBundleOptionWindow.ShowWindow();
        }

        [MenuItem("Lite/AssetBundle/BuilderNoCollect %#K", false, 2)]
        private static void BuildAssetsNoCollect()
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                EditorUtility.DisplayDialog("Lite", "building is running", "Ok");
                return;
            }

            BuildAsset(EditorUserBuildSettings.activeBuildTarget, BuildAssetBundleOptions.None, false);
        }

        [MenuItem("Lite/AssetBundle/Collect")]
        private static void CollectBundleInfo()
        {
            foreach (var ID in Selection.assetGUIDs)
            {
                var Path = AssetDatabase.GUIDToAssetPath(ID);
                if (Directory.Exists(Path))
                {
                    var FullPath = PathHelper.UnifyPath($"{Application.dataPath}{Path.Substring("Assets".Length)}");
                    var AssetsList = CollectAllAssetBundlePath($"{Application.dataPath}/{LiteConfigure.StandaloneAssetsName}/", FullPath);
                    ConfigurationAssetBundle(AssetsList);
                }
            }
        }

        public static void BuildAsset(BuildTarget Target, BuildAssetBundleOptions Options, bool IsCollectInfo)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            if (IsCollectInfo)
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();

                LuaFileList_.Clear();
                var RootPath = $"{Application.dataPath}/{LiteConfigure.StandaloneAssetsName}/";
                var AssetsList = CollectAllAssetBundlePath(RootPath, RootPath);
                AssetsList = HandleAllLuaFile(AssetsList);
                ConfigurationAssetBundle(AssetsList);
            }

            // Start Build
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, Options, Target);
            File.Delete(Path.Combine(Application.streamingAssetsPath, LiteConfigure.AssetBundleManifestName));
            File.Copy(
                Path.Combine(Application.streamingAssetsPath, "StreamingAssets"),
                Path.Combine(Application.streamingAssetsPath, LiteConfigure.AssetBundleManifestName));
            File.Delete(Path.Combine(Application.streamingAssetsPath, "StreamingAssets"));

            if (IsCollectInfo)
            {
                DeleteAllLuaFile();
            }

            EditorUtility.DisplayDialog("Lite", "building done", "Ok");
            AssetDatabase.Refresh();
        }

        private static void DeleteAllLuaFile()
        {
            foreach (var LuaFile in LuaFileList_)
            {
                File.Delete(LuaFile);
                File.Delete($"{LuaFile}.meta");
            }

            LuaFileList_.Clear();
        }

        private static void HandleLuaFile(string FilePath)
        {
            var OldFilePath = $"Assets/{LiteConfigure.StandaloneAssetsName}/{FilePath}";
            var NewFilePath = $"Assets/{LiteConfigure.StandaloneAssetsName}/{FilePath}.bytes";
            LuaFileList_.Add(NewFilePath);
            File.Copy(OldFilePath, NewFilePath, true);
        }

        private static List<string> HandleAllLuaFile(List<string> AssetsList)
        {
            var FileList = new List<string>();

            foreach (var Asset in AssetsList)
            {
                if (Asset.EndsWith(".lua"))
                {
                    HandleLuaFile(Asset);
                    FileList.Add($"{Asset}.bytes");
                }
                else
                {
                    FileList.Add(Asset);
                }
            }

            AssetDatabase.Refresh();
            return FileList;
        }

        private static List<string> CollectAllAssetBundlePath(string RootPath, string CurrentPath)
        {
            CurrentPath = PathHelper.UnifyPath(CurrentPath);
            var Result = new List<string>();

            var Files = Directory.GetFiles(CurrentPath);
            foreach (var FilePath in Files)
            {
                if (FilePath.EndsWith(".meta"))
                {
                    continue;
                }

                Result.Add(FilePath.Substring(RootPath.Length));
            }

            var Dirs = Directory.GetDirectories(CurrentPath);
            foreach (var DirPath in Dirs)
            {
                var FileList = CollectAllAssetBundlePath(RootPath, DirPath);
                Result.AddRange(FileList);
            }

            return Result;
        }

        private static void ConfigurationAssetBundle(List<string> AssetsList)
        {
            foreach (var AssetPath in AssetsList)
            {
                SetAssetBundleName(AssetPath);
            }
        }

        private static void SetAssetBundleName(string ResPath)
        {
            var Importer = AssetImporter.GetAtPath($"Assets/{LiteConfigure.StandaloneAssetsName}/{ResPath}");
            if (Importer == null)
            {
                LLogger.LError($"unexpected asset path : {ResPath}");
                return;
            }

            /*var Index = ResPath.LastIndexOf('.');
            if (Index == -1)
            {
                LLogger.LError($"unexpected asset path : {ResPath}");
                return;
            }
    
            var BundleName = ResPath.Substring(0, Index);*/

            /*var BundleName = ResPath;

            if (Importer is TextureImporter TexImporter)
            {
                var Index = ResPath.LastIndexOf('.');
                BundleName = ResPath.Substring(0, Index);

                if (TexImporter.textureType == TextureImporterType.Sprite)
                {
                    BundleName = $"{BundleName}.sprite";
                }
                else
                {
                    BundleName = $"{BundleName}.texture";
                }
            }*/

            var BundleName = ResPath;

            Importer.assetBundleName = BundleName;
            Importer.assetBundleVariant = string.Empty;
        }

        public static void CleanAllBundle()
        {
            var RootPath = $"{Application.dataPath}/{LiteConfigure.StandaloneAssetsName}/";
            var AssetList = CollectAllAssetBundlePath(RootPath, RootPath);
            foreach (var Asset in AssetList)
            {
                var Importer = AssetImporter.GetAtPath($"Assets/{LiteConfigure.StandaloneAssetsName}/{Asset}");
                if (Importer != null)
                {
                    Importer.assetBundleName = string.Empty;
                    Importer.assetBundleVariant = string.Empty;
                }
            }

            Directory.Delete(Application.streamingAssetsPath, true);
            Directory.CreateDirectory(Application.streamingAssetsPath);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            LLogger.LWarning("StreamingAssets Clean");
        }

        public static void DeleteStreamingAssets()
        {
            Directory.Delete(Application.streamingAssetsPath, true);
            Directory.CreateDirectory(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
            LLogger.LWarning("StreamingAssets Remove");
        }
    }
}