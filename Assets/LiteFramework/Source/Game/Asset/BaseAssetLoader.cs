using System;
using System.Collections;
using System.Collections.Generic;
using LiteFramework.Helper;
using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Log;

namespace LiteFramework.Game.Asset
{
    internal abstract class BaseAssetLoader : IAssetLoader
    {
        protected Dictionary<string, BaseAssetCache> AssetCacheList_ = new Dictionary<string, BaseAssetCache>();
        protected Dictionary<string, List<Action>> AssetLoadCallbackList_ = new Dictionary<string, List<Action>>();
        protected Dictionary<int, string> AssetPathCacheList_ = new Dictionary<int, string>();

        protected BaseAssetLoader()
        {
        }

        public virtual bool Startup()
        {
            AssetCacheList_.Clear();
            AssetLoadCallbackList_.Clear();
            AssetPathCacheList_.Clear();

            return true;
        }

        public virtual void Shutdown()
        {
            if (AssetCacheList_.Count > 0)
            {
                foreach (var Cache in AssetCacheList_)
                {
                    Cache.Value.Unload();
                }

                AssetCacheList_.Clear();
            }

            AssetLoadCallbackList_.Clear();
            AssetPathCacheList_.Clear();

            UnityEngine.Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public virtual void Tick(float DeltaTime)
        {
        }

        protected AssetCacheType GetAssetTypeWithName(string AssetPath)
        {
            var Ext = PathHelper.GetFileExt(AssetPath);

            switch (Ext)
            {
                case ".prefab":
                    return AssetCacheType.Prefab;
                case ".bytes":
                case ".lua":
                    return AssetCacheType.Data;
                default:
                    return AssetCacheType.Asset;
            }
        }

        protected abstract BaseAssetCache CreateAssetCache<T>(AssetCacheType AssetType, string AssetPath) where T : UnityEngine.Object;

        public bool AssetCacheExisted(string AssetPath)
        {
            AssetPath = AssetPath.ToLower();

            if (AssetCacheList_.ContainsKey(AssetPath) && AssetCacheList_[AssetPath].IsLoad)
            {
                return true;
            }

            return false;
        }

        public void PreloadAsset<T>(string AssetPath) where T : UnityEngine.Object
        {
            var AssetType = GetAssetTypeWithName(AssetPath);
            LoadAssetSync<T>(AssetType, AssetPath);
        }

        protected virtual bool LoadAssetAsync<T>(AssetCacheType AssetType, string AssetPath, Action Callback = null) where T : UnityEngine.Object
        {
            AssetPath = AssetPath.ToLower();
            if (AssetCacheExisted(AssetPath))
            {
                Callback?.Invoke();
                return true;
            }

            if (!AssetLoadCallbackList_.ContainsKey(AssetPath))
            {
                AssetLoadCallbackList_.Add(AssetPath, new List<Action> {Callback});

                return LoadAssetCacheCompletedAsync<T>(AssetType, AssetPath, () =>
                {
                    foreach (var LoadCallback in AssetLoadCallbackList_[AssetPath])
                    {
                        LoadCallback?.Invoke();
                    }

                    AssetLoadCallbackList_.Remove(AssetPath);
                });
            }
            else
            {
                AssetLoadCallbackList_[AssetPath].Add(Callback);
            }

            return true;
        }

        protected virtual BaseAssetCache LoadAssetSync<T>(AssetCacheType AssetType, string AssetPath) where T : UnityEngine.Object
        {
            AssetPath = AssetPath.ToLower();
            if (AssetCacheExisted(AssetPath))
            {
                return AssetCacheList_[AssetPath];
            }

            return LoadAssetCacheCompletedSync<T>(AssetType, AssetPath);
        }

        private bool LoadAssetCacheCompletedAsync<T>(AssetCacheType AssetType, string AssetPath, Action Callback = null) where T : UnityEngine.Object
        {
            var Cache = CreateAssetCache<T>(AssetType, AssetPath);
            if (Cache == null)
            {
                Callback?.Invoke();
                return false;
            }

            AssetCacheList_.Add(AssetPath, Cache);
            LoadAssetCacheDependenciesAsync<T>(Cache, () => { TaskManager.AddTask(LoadAssetCacheAsync<T>(Cache), () => { Callback?.Invoke(); }); });

            return true;
        }

        private BaseAssetCache LoadAssetCacheCompletedSync<T>(AssetCacheType AssetType, string AssetPath) where T : UnityEngine.Object
        {
            var Cache = CreateAssetCache<T>(AssetType, AssetPath);
            if (Cache == null)
            {
                return null;
            }

            AssetCacheList_.Add(AssetPath, Cache);
            LoadAssetCacheDependenciesSync<T>(Cache);
            return LoadAssetCacheSync<T>(Cache);
        }

        private IEnumerator LoadAssetCacheAsync<T>(BaseAssetCache Cache) where T : UnityEngine.Object
        {
            if (!AssetCacheList_.ContainsKey(Cache.AssetPath))
            {
                yield break;
            }

            yield return TaskManager.WaitTask(Cache.LoadAsync());

            if (!Cache.IsLoad)
            {
                AssetCacheList_.Remove(Cache.AssetPath);
            }
        }

        private BaseAssetCache LoadAssetCacheSync<T>(BaseAssetCache Cache) where T : UnityEngine.Object
        {
            if (!AssetCacheList_.ContainsKey(Cache.AssetPath))
            {
                return null;
            }

            Cache.LoadSync();

            if (!Cache.IsLoad)
            {
                AssetCacheList_.Remove(Cache.AssetPath);
                Cache = null;
            }

            return Cache;
        }

        private void LoadAssetCacheDependenciesAsync<T>(BaseAssetCache Cache, Action Callback = null) where T : UnityEngine.Object
        {
            var LoadCompletedCount = 0;
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                Callback?.Invoke();
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var AssetPath = Dependency;
                var AssetType = GetAssetTypeWithName(AssetPath);
                LoadAssetAsync<T>(AssetType, AssetPath, () =>
                {
                    Cache.AddDependencyCache(AssetCacheList_[AssetPath]);
                    LoadCompletedCount++;

                    if (LoadCompletedCount >= Dependencies.Length)
                    {
                        Callback?.Invoke();
                    }
                });
            }
        }

        private void LoadAssetCacheDependenciesSync<T>(BaseAssetCache Cache) where T : UnityEngine.Object
        {
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var AssetPath = Dependency;
                var AssetType = GetAssetTypeWithName(AssetPath);

                var DependencyAsset = LoadAssetSync<T>(AssetType, AssetPath);
                if (DependencyAsset != null)
                {
                    Cache.AddDependencyCache(DependencyAsset);
                }
            }
        }

        public void CreateAssetAsync<T>(string AssetPath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var IsLoaded = LoadAssetAsync<T>(AssetCacheType.Asset, AssetPath, () => { Callback?.Invoke(CreateAssetSync<T>(AssetPath, AssetName)); });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreateAssetAsync<T>(string AssetPath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            CreateAssetAsync<T>(AssetPath, AssetName, Callback);
        }

        public T CreateAssetSync<T>(string AssetPath, string AssetName) where T : UnityEngine.Object
        {
            var AssetType = GetAssetTypeWithName(AssetPath);

            T Asset = null;
            AssetPath = AssetPath.ToLower();

            if (AssetType == AssetCacheType.Asset)
            {
                AssetName = $"{AssetName.ToLower()}_{typeof(T).Name.ToLower()}";
            }

            BaseAssetCache Cache = null;
            if (!AssetCacheList_.ContainsKey(AssetPath))
            {
                Cache = LoadAssetSync<UnityEngine.Object>(AssetType, AssetPath);
            }
            else
            {
                Cache = AssetCacheList_[AssetPath];
            }

            if (Cache != null)
            {
                Asset = Cache.CreateAsset(AssetName) as T;

                if (Asset != null)
                {
                    if (!AssetPathCacheList_.ContainsKey(Asset.GetInstanceID()))
                    {
                        AssetPathCacheList_.Add(Asset.GetInstanceID(), AssetPath);
                    }
                }
                else
                {
                    LLogger.LWarning($"Can't Create Asset : {AssetPath} - {AssetName}");
                }
            }

            return Asset;
        }

        public T CreateAssetSync<T>(string AssetPath) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            return CreateAssetSync<T>(AssetPath, AssetName);
        }

        public void CreatePrefabAsync(string AssetPath, string AssetName, Action<UnityEngine.GameObject> Callback = null)
        {
            var IsLoaded = LoadAssetAsync<UnityEngine.Object>(AssetCacheType.Prefab, AssetPath,
                () => { Callback?.Invoke(CreatePrefabSync(AssetPath, AssetName)); });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreatePrefabAsync(string AssetPath, Action<UnityEngine.GameObject> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            CreatePrefabAsync(AssetPath, AssetName, Callback);
        }

        public UnityEngine.GameObject CreatePrefabSync(string AssetPath, string AssetName)
        {
            return CreateAssetSync<UnityEngine.GameObject>(AssetPath, AssetName);
        }

        public UnityEngine.GameObject CreatePrefabSync(string AssetPath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            return CreatePrefabSync(AssetPath, AssetName);
        }

        public void CreateDataAsync(string AssetPath, string AssetName, Action<byte[]> Callback = null)
        {
            var IsLoaded = LoadAssetAsync<UnityEngine.TextAsset>(AssetCacheType.Data, AssetPath,
                () => { Callback?.Invoke(CreateDataSync(AssetPath, AssetName)); });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreateDataAsync(string AssetPath, Action<byte[]> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            CreateDataAsync(AssetPath, AssetName, Callback);
        }

        public byte[] CreateDataSync(string AssetPath, string AssetName)
        {
            UnityEngine.TextAsset Asset = CreateAssetSync<UnityEngine.TextAsset>(AssetPath, AssetName);
            return Asset?.bytes;
        }

        public byte[] CreateDataSync(string AssetPath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(AssetPath);
            return CreateDataSync(AssetPath, AssetName);
        }

        public void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            if (Asset == null)
            {
                return;
            }

            if (AssetPathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var AssetPath = AssetPathCacheList_[Asset.GetInstanceID()];
                AssetPathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetCacheExisted(AssetPath))
                {
                    AssetCacheList_[AssetPath].DeleteAsset(Asset);
                }
            }
            else if (typeof(T) == typeof(UnityEngine.GameObject))
            {
                UnityEngine.Object.Destroy(Asset);
            }
        }

        public void DeleteAsset(UnityEngine.GameObject Asset)
        {
            DeleteAsset<UnityEngine.GameObject>(Asset);
        }

        public void DeleteUnusedAsset()
        {
            var RemoveList = new List<string>();

            foreach (var Cache in AssetCacheList_)
            {
                if (Cache.Value.Unused)
                {
                    RemoveList.Add(Cache.Key);
                }
            }

            if (RemoveList.Count > 0)
            {
                foreach (var Key in RemoveList)
                {
                    AssetCacheList_[Key].Unload();
                    AssetCacheList_.Remove(Key);
                }

                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}