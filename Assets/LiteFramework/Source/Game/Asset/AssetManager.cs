using System;
using UnityEngine;

namespace LiteFramework.Game.Asset
{
    public static class AssetManager
    {
        private static IAssetLoader Loader_ = null;

        public static bool Startup()
        {
#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
            Loader_ = new AssetInternalLoader();
#else
            Loader_ = new AssetBundleLoader();
#endif
            return Loader_.Startup();
        }

        public static void Shutdown()
        {
            Loader_.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            Loader_.Tick(DeltaTime);
        }

        public static bool AssetCacheExisted(string AssetPath)
        {
            return Loader_.AssetCacheExisted(AssetPath);
        }

        public static void PreloadedAsset<T>(string AssetPath, Action<bool> Callback) where T : UnityEngine.Object
        {
            Loader_.PreloadedAsset<T>(AssetPath, Callback);
        }

        public static void CreateAssetAsync<T>(AssetUri Uri, Action<T> Callback = null) where T : UnityEngine.Object
        {
            Loader_.CreateAssetAsync<T>(Uri, Callback);
        }

        public static T CreateAssetSync<T>(AssetUri Uri) where T : UnityEngine.Object
        {
            return Loader_.CreateAssetSync<T>(Uri);
        }

        public static void CreatePrefabAsync(AssetUri Uri, Action<GameObject> Callback = null)
        {
            Loader_.CreatePrefabAsync(Uri, Callback);
        }

        public static GameObject CreatePrefabSync(AssetUri Uri)
        {
            return Loader_.CreatePrefabSync(Uri);
        }

        public static void CreateDataAsync(AssetUri Uri, Action<byte[]> Callback = null)
        {
            Loader_.CreateDataAsync(Uri, Callback);
        }

        public static byte[] CreateDataSync(AssetUri Uri)
        {
            return Loader_.CreateDataSync(Uri);
        }

        public static void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            Loader_.DeleteAsset<T>(Asset);
        }

        public static void DeleteAsset(GameObject Asset)
        {
            Loader_.DeleteAsset(Asset);
        }

        public static void DeleteUnusedAsset()
        {
            Loader_.DeleteUnusedAsset();
        }
    }
}