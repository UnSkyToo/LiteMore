using System;
using System.Collections;
using System.Collections.Generic;
using LiteFramework.Helper;
using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Log;
using LiteFramework.Core.ObjectPool;

namespace LiteFramework.Game.Asset
{
    /*internal abstract class BaseAssetLoader : IAssetLoader
    {
        protected readonly Dictionary<string, BaseAssetCache> AssetCacheList_ = new Dictionary<string, BaseAssetCache>();
        protected readonly Dictionary<string, List<Action>> AssetLoadCallbackList_ = new Dictionary<string, List<Action>>();
        protected readonly Dictionary<int, string> AssetPathCacheList_ = new Dictionary<int, string>();

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
            UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public virtual void Tick(float DeltaTime)
        {
        }

        protected AssetBundleType GetAssetBundleTypeWithName(string BundlePath)
        {
            var Ext = PathHelper.GetFileExt(BundlePath);

            switch (Ext)
            {
                case ".prefab":
                    return AssetBundleType.Prefab;
                case ".bytes":
                    return AssetBundleType.Data;
                default:
                    return AssetBundleType.Asset;
            }
        }

        protected abstract BaseAssetCache CreateAssetCache<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object;

        public bool AssetBundleCacheExisted(string BundlePath)
        {
            BundlePath = BundlePath.ToLower();

            if (AssetCacheList_.ContainsKey(BundlePath) && AssetCacheList_[BundlePath].IsLoad)
            {
                return true;
            }

            return false;
        }

        protected bool LoadAssetBundleAsync<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                Callback?.Invoke();
                return true;
            }

            if (!AssetLoadCallbackList_.ContainsKey(BundlePath))
            {
                AssetLoadCallbackList_.Add(BundlePath, new List<Action> { Callback });

                return LoadAssetBundleCacheCompletedAsync<T>(BundleType, BundlePath, () =>
                {
                    foreach (var LoadCallback in AssetLoadCallbackList_[BundlePath])
                    {
                        LoadCallback?.Invoke();
                    }
                    AssetLoadCallbackList_.Remove(BundlePath);
                });
            }
            else
            {
                AssetLoadCallbackList_[BundlePath].Add(Callback);
            }

            return true;
        }

        private AssetBundleCacheBase LoadAssetBundleSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                return AssetBundleCacheList_[BundlePath];
            }

            if (!AssetBundlePathList_.Contains(BundlePath))
            {
                return null;
            }

            return LoadAssetBundleCacheCompletedSync<T>(BundleType, BundlePath);
        }

        private bool LoadAssetBundleCacheCompletedAsync<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            var Cache = CreateAssetBundleCache<T>(BundleType, BundlePath);
            if (Cache == null)
            {
                Callback?.Invoke();
                return false;
            }

            AssetBundleCacheList_.Add(BundlePath, Cache);
            LoadAssetBundleCacheDependenciesAsync<T>(Cache, () =>
            {
                TaskManager.AddTask(LoadAssetBundleCacheAsync<T>(Cache), () =>
                {
                    Callback?.Invoke();
                });
            });

            return true;
        }

        private AssetBundleCacheBase LoadAssetBundleCacheCompletedSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            var Cache = CreateAssetBundleCache<T>(BundleType, BundlePath);
            if (Cache == null)
            {
                return null;
            }

            AssetBundleCacheList_.Add(BundlePath, Cache);
            LoadAssetBundleCacheDependenciesSync<T>(Cache);
            return LoadAssetBundleCacheSync<T>(Cache); ;
        }

        private IEnumerator LoadAssetBundleCacheAsync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            if (!AssetBundleCacheList_.ContainsKey(Cache.BundlePath))
            {
                yield break;
            }

            yield return TaskManager.WaitTask(Cache.LoadAsync());

            if (!Cache.IsLoad)
            {
                AssetBundleCacheList_.Remove(Cache.BundlePath);
            }
            else
            {
                //AssetBundleCacheList_[Cache.BundlePath].UnloadAssetBundle();
            }
        }

        private AssetBundleCacheBase LoadAssetBundleCacheSync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            if (!AssetBundleCacheList_.ContainsKey(Cache.BundlePath))
            {
                return null;
            }

            Cache.LoadSync();

            if (!Cache.IsLoad)
            {
                AssetBundleCacheList_.Remove(Cache.BundlePath);
                Cache = null;
            }
            else
            {
                //AssetBundleCacheList_[Cache.BundlePath].UnloadAssetBundle();
            }

            return Cache;
        }

        private void LoadAssetBundleCacheDependenciesAsync<T>(AssetBundleCacheBase Cache, Action Callback = null) where T : UnityEngine.Object
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
                var BundlePath = Dependency;
                var BundleType = GetAssetBundleTypeWithName(BundlePath);
                LoadAssetBundleAsync<T>(BundleType, BundlePath, () =>
                {
                    Cache.AddDependencyCache(AssetBundleCacheList_[BundlePath]);
                    LoadCompletedCount++;

                    if (LoadCompletedCount >= Dependencies.Length)
                    {
                        Callback?.Invoke();
                    }
                });
            }
        }

        private void LoadAssetBundleCacheDependenciesSync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var BundlePath = Dependency;
                var BundleType = GetAssetBundleTypeWithName(BundlePath);

                var DependencyBundle = LoadAssetBundleSync<T>(BundleType, BundlePath);
                if (DependencyBundle != null)
                {
                    Cache.AddDependencyCache(DependencyBundle);
                }
            }
        }

        public void CreateAssetAsync<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var IsLoaded = LoadAssetBundleAsync<T>(AssetBundleType.Asset, BundlePath, () =>
            {
                Callback?.Invoke(CreateAssetSync<T>(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreateAssetAsync<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateAssetAsync<T>(BundlePath, AssetName, Callback);
        }

        public T CreateAssetSync<T>(string BundlePath, string AssetName) where T : UnityEngine.Object
        {
            T Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = $"{AssetName.ToLower()}_{typeof(T).Name.ToLower()}";

            AssetBundleCache<UnityEngine.Object> AssetCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                AssetCache = LoadAssetBundleSync<UnityEngine.Object>(AssetBundleType.Asset, BundlePath) as AssetBundleCache<UnityEngine.Object>;
            }
            else
            {
                AssetCache = AssetBundleCacheList_[BundlePath] as AssetBundleCache<UnityEngine.Object>;
            }

            if (AssetCache != null)
            {
                Asset = AssetCache.CreateAsset(AssetName) as T;

                if (Asset != null)
                {
                    if (!AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
                    {
                        AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                    }
                }
                else
                {
                    LLogger.LWarning($"Can't Create Asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset;
        }

        public T CreateAssetSync<T>(string BundlePath) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreateAssetSync<T>(BundlePath, AssetName);
        }

        public void CreatePrefabAsync(string BundlePath, string AssetName, Action<UnityEngine.GameObject> Callback = null)
        {
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.Object>(AssetBundleType.Prefab, BundlePath, () =>
            {
                Callback?.Invoke(CreatePrefabSync(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreatePrefabAsync(string BundlePath, Action<UnityEngine.GameObject> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreatePrefabAsync(BundlePath, AssetName, Callback);
        }

        public UnityEngine.GameObject CreatePrefabSync(string BundlePath, string AssetName)
        {
            UnityEngine.GameObject Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            PrefabBundleCache PrefabCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                PrefabCache = LoadAssetBundleSync<UnityEngine.Object>(AssetBundleType.Prefab, BundlePath) as PrefabBundleCache;
            }
            else
            {
                PrefabCache = AssetBundleCacheList_[BundlePath] as PrefabBundleCache;
            }

            if (PrefabCache != null)
            {
                Asset = PrefabCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                }
                else
                {
                    LLogger.LWarning($"can't create asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset;
        }

        public UnityEngine.GameObject CreatePrefabSync(string BundlePath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreatePrefabSync(BundlePath, AssetName);
        }

        public void CreateDataAsync(string BundlePath, string AssetName, Action<byte[]> Callback = null)
        {
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.TextAsset>(AssetBundleType.Data, BundlePath, () =>
            {
                Callback?.Invoke(CreateDataSync(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateDataAsync(BundlePath, AssetName, Callback);
        }

        public byte[] CreateDataSync(string BundlePath, string AssetName)
        {
            UnityEngine.TextAsset Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            DataBundleCache DataCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                DataCache = LoadAssetBundleSync<UnityEngine.TextAsset>(AssetBundleType.Data, BundlePath) as DataBundleCache;
            }
            else
            {
                DataCache = AssetCacheList_[BundlePath] as DataBundleCache;
            }

            if (DataCache != null)
            {
                Asset = DataCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    AssetPathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                }
                else
                {
                    LLogger.LWarning($"can't create asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset?.bytes;
        }

        public byte[] CreateDataSync(string BundlePath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreateDataSync(BundlePath, AssetName);
        }

        public void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            if (Asset == null)
            {
                return;
            }

            if (AssetPathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var BundlePath = AssetPathCacheList_[Asset.GetInstanceID()];
                AssetPathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetBundleCacheExisted(BundlePath))
                {
                    OnDeleteAsset(Asset);
                }
            }
        }

        protected abstract void OnDeleteAsset<T>(T Asset);

        public void DeleteUnusedAssetBundle()
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
    }*/
}