using System;
using UnityEngine;

namespace LiteFramework.Game.Asset
{
    internal interface IAssetLoader
    {
        bool Startup();
        void Shutdown();
        void Tick(float DeltaTime);

        bool AssetCacheExisted(string AssetPath);
        void PreloadedAsset<T>(string AssetPath, Action<bool> Callback) where T : UnityEngine.Object;

        void CreateAssetAsync<T>(AssetUri Uri, Action<T> Callback = null) where T : UnityEngine.Object;
        T CreateAssetSync<T>(AssetUri Uri) where T : UnityEngine.Object;

        void CreatePrefabAsync(AssetUri Uri, Action<GameObject> Callback = null);
        GameObject CreatePrefabSync(AssetUri Uri);

        void CreateDataAsync(AssetUri Uri, Action<byte[]> Callback = null);
        byte[] CreateDataSync(AssetUri Uri);

        void DeleteAsset<T>(T Asset) where T : UnityEngine.Object;
        void DeleteAsset(GameObject Asset);
        void DeleteUnusedAsset();
    }
}