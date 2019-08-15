using System;
using UnityEngine;

namespace LiteFramework.Game.Asset
{
    internal interface IAssetLoader
    {
        bool Startup();
        void Shutdown();
        void Tick(float DeltaTime);

        void CreateAssetAsync<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object;
        void CreateAssetAsync<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object;
        T CreateAssetSync<T>(string BundlePath, string AssetName) where T : UnityEngine.Object;
        T CreateAssetSync<T>(string BundlePath) where T : UnityEngine.Object;

        void CreatePrefabAsync(string BundlePath, string AssetName, Action<GameObject> Callback = null);
        void CreatePrefabAsync(string BundlePath, Action<GameObject> Callback = null);
        GameObject CreatePrefabSync(string BundlePath, string AssetName);
        GameObject CreatePrefabSync(string BundlePath);

        void CreateDataAsync(string BundlePath, string AssetName, Action<byte[]> Callback = null);
        void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null);
        byte[] CreateDataSync(string BundlePath, string AssetName);
        byte[] CreateDataSync(string BundlePath);

        void DeleteAsset<T>(T Asset) where T : UnityEngine.Object;
        void DeleteAsset(GameObject Asset);
        void DeleteUnusedAsset();
    }
}