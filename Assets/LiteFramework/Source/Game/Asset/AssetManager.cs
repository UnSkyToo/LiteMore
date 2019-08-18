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

        public static void PreloadAsset<T>(string AssetPath) where T : UnityEngine.Object
        {
            Loader_.PreloadAsset<T>(AssetPath);
        }

        public static void CreateAssetAsync<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            Loader_.CreateAssetAsync<T>(BundlePath, AssetName, Callback);
        }

        public static void CreateAssetAsync<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            Loader_.CreateAssetAsync(BundlePath, Callback);
        }

        public static T CreateAssetSync<T>(string BundlePath, string AssetName) where T : UnityEngine.Object
        {
            return Loader_.CreateAssetSync<T>(BundlePath, AssetName);
        }

        public static T CreateAssetSync<T>(string BundlePath) where T : UnityEngine.Object
        {
            return Loader_.CreateAssetSync<T>(BundlePath);
        }

        public static void CreatePrefabAsync(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            Loader_.CreatePrefabAsync(BundlePath, AssetName, Callback);
        }

        public static void CreatePrefabAsync(string BundlePath, Action<GameObject> Callback = null)
        {
            Loader_.CreatePrefabAsync(BundlePath, Callback);
        }

        public static GameObject CreatePrefabSync(string BundlePath, string AssetName)
        {
            return Loader_.CreatePrefabSync(BundlePath, AssetName);
        }

        public static GameObject CreatePrefabSync(string BundlePath)
        {
            return Loader_.CreatePrefabSync(BundlePath);
        }

        public static void CreateDataAsync(string BundlePath, string AssetName, Action<byte[]> Callback = null)
        {
            Loader_.CreateDataAsync(BundlePath, AssetName, Callback);
        }

        public static void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null)
        {
            Loader_.CreateDataAsync(BundlePath, Callback);
        }

        public static byte[] CreateDataSync(string BundlePath, string AssetName)
        {
            return Loader_.CreateDataSync(BundlePath, AssetName);
        }

        public static byte[] CreateDataSync(string BundlePath)
        {
            return Loader_.CreateDataSync(BundlePath);
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