using System;
using UnityEngine;

#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
using AssetMgr = LiteFramework.Game.Asset.AssetInternalManager;
#else
using AssetMgr = LiteFramework.Game.Asset.AssetBundleManager;
#endif

namespace LiteFramework.Game.Asset
{
    public static class AssetManager
    {
        public static bool Startup()
        {
            return AssetMgr.Startup();
        }

        public static void Shutdown()
        {
            AssetMgr.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            AssetMgr.Tick(DeltaTime);
        }

        public static void CreateAssetAsync<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            AssetMgr.CreateAssetAsync<T>(BundlePath, AssetName, Callback);
        }

        public static void CreateAssetAsync<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            AssetMgr.CreateAssetAsync(BundlePath, Callback);
        }

        public static T CreateAssetSync<T>(string BundlePath, string AssetName) where T : UnityEngine.Object
        {
            return AssetMgr.CreateAssetSync<T>(BundlePath, AssetName);
        }

        public static T CreateAssetSync<T>(string BundlePath) where T : UnityEngine.Object
        {
            return AssetMgr.CreateAssetSync<T>(BundlePath);
        }

        public static void CreatePrefabAsync(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            AssetMgr.CreatePrefabAsync(BundlePath, AssetName, Callback);
        }

        public static void CreatePrefabAsync(string BundlePath, Action<GameObject> Callback = null)
        {
            AssetMgr.CreatePrefabAsync(BundlePath, Callback);
        }

        public static GameObject CreatePrefabSync(string BundlePath, string AssetName)
        {
            return AssetMgr.CreatePrefabSync(BundlePath, AssetName);
        }

        public static GameObject CreatePrefabSync(string BundlePath)
        {
            return AssetMgr.CreatePrefabSync(BundlePath);
        }

        public static void CreateDataAsync(string BundlePath, string AssetName, Action<byte[]> Callback = null)
        {
            AssetMgr.CreateDataAsync(BundlePath, AssetName, Callback);
        }

        public static void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null)
        {
            AssetMgr.CreateDataAsync(BundlePath, Callback);
        }

        public static byte[] CreateDataSync(string BundlePath, string AssetName)
        {
            return AssetMgr.CreateDataSync(BundlePath, AssetName);
        }

        public static byte[] CreateDataSync(string BundlePath)
        {
            return AssetMgr.CreateDataSync(BundlePath);
        }

        public static void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            AssetMgr.DeleteAsset<T>(Asset);
        }

        public static void DeleteAsset(GameObject Asset)
        {
            AssetMgr.DeleteAsset(Asset);
        }

        public static void DeleteUnusedAssetBundle()
        {
            AssetMgr.DeleteUnusedAssetBundle();
        }
    }
}