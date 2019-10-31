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
        protected Dictionary<string, List<Action<bool>>> AssetLoadCallbackList_ = new Dictionary<string, List<Action<bool>>>();
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

        protected AssetCacheType GetAssetTypeWithName<T>(string AssetPath)
        {
            var Ext = PathHelper.GetFileExt(AssetPath);

            switch (Ext)
            {
                case ".prefab":
                    return AssetCacheType.Prefab;
                case ".bytes":
                case ".lua":
                    return AssetCacheType.Data;
                case "":
                    if (typeof(T) == typeof(UnityEngine.GameObject))
                    {
                        return AssetCacheType.Prefab;
                    }

                    if (typeof(T) == typeof(UnityEngine.TextAsset))
                    {
                        return AssetCacheType.Data;
                    }

                    return AssetCacheType.Asset;
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

        public void PreloadedAsset<T>(string AssetPath, Action<bool> Callback) where T : UnityEngine.Object
        {
            var AssetType = GetAssetTypeWithName<T>(AssetPath);
            LoadAssetAsync<T>(AssetType, AssetPath, Callback);
        }

        protected virtual void LoadAssetAsync<T>(AssetCacheType AssetType, string AssetPath, Action<bool> Callback = null) where T : UnityEngine.Object
        {
            AssetPath = AssetPath.ToLower();
            if (AssetCacheExisted(AssetPath))
            {
                Callback?.Invoke(true);
                return;
            }

            if (!AssetLoadCallbackList_.ContainsKey(AssetPath))
            {
                AssetLoadCallbackList_.Add(AssetPath, new List<Action<bool>> {Callback});

                LoadAssetCacheCompletedAsync<T>(AssetType, AssetPath, (IsLoaded) =>
                {
                    foreach (var LoadCallback in AssetLoadCallbackList_[AssetPath])
                    {
                        LoadCallback?.Invoke(IsLoaded);
                    }

                    AssetLoadCallbackList_.Remove(AssetPath);
                });
            }
            else
            {
                AssetLoadCallbackList_[AssetPath].Add(Callback);
            }
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

        private void LoadAssetCacheCompletedAsync<T>(AssetCacheType AssetType, string AssetPath, Action<bool> Callback = null) where T : UnityEngine.Object
        {
            var Cache = CreateAssetCache<T>(AssetType, AssetPath);
            if (Cache == null)
            {
                Callback?.Invoke(false);
                return;
            }

            AssetCacheList_.Add(AssetPath, Cache);
            LoadAssetCacheDependenciesAsync<T>(Cache, (IsLoaded) =>
            {
                if (!IsLoaded)
                {
                    Callback?.Invoke(false);
                    return;
                }

                TaskManager.AddTask(LoadAssetCacheAsync<T>(Cache), () => { Callback?.Invoke(true); });
            });
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

        private void LoadAssetCacheDependenciesAsync<T>(BaseAssetCache Cache, Action<bool> Callback = null) where T : UnityEngine.Object
        {
            var LoadCompletedCount = 0;
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                Callback?.Invoke(true);
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var AssetPath = Dependency;
                var AssetType = GetAssetTypeWithName<T>(AssetPath);
                LoadAssetAsync<T>(AssetType, AssetPath, (IsLoaded) =>
                {
                    if (!IsLoaded)
                    {
                        Callback?.Invoke(false);
                        return;
                    }

                    Cache.AddDependencyCache(AssetCacheList_[AssetPath]);
                    LoadCompletedCount++;

                    if (LoadCompletedCount >= Dependencies.Length)
                    {
                        Callback?.Invoke(true);
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
                var AssetType = GetAssetTypeWithName<T>(AssetPath);

                var DependencyAsset = LoadAssetSync<T>(AssetType, AssetPath);
                if (DependencyAsset != null)
                {
                    Cache.AddDependencyCache(DependencyAsset);
                }
            }
        }

        public void CreateAssetAsync<T>(AssetUri Uri, Action<T> Callback = null) where T : UnityEngine.Object
        {
            LoadAssetAsync<T>(AssetCacheType.Asset, Uri.AssetPath, (IsLoaded) =>
            {
                if (!IsLoaded)
                {
                    Callback?.Invoke(null);
                    return;
                }

                Callback?.Invoke(CreateAssetSync<T>(Uri));
            });
        }

        public T CreateAssetSync<T>(AssetUri Uri) where T : UnityEngine.Object
        {
            var AssetType = GetAssetTypeWithName<T>(Uri.AssetPath);

            T Asset = null;

            var AssetName = Uri.AssetName;
            if (AssetType == AssetCacheType.Asset)
            {
                AssetName = $"{AssetName}_{typeof(T).Name.ToLower()}";
            }

            BaseAssetCache Cache = null;
            if (!AssetCacheList_.ContainsKey(Uri.AssetPath))
            {
                Cache = LoadAssetSync<UnityEngine.Object>(AssetType, Uri.AssetPath);
            }
            else
            {
                Cache = AssetCacheList_[Uri.AssetPath];
            }

            if (Cache != null)
            {
                Asset = Cache.CreateAsset(AssetName) as T;

                if (Asset != null)
                {
                    if (!AssetPathCacheList_.ContainsKey(Asset.GetInstanceID()))
                    {
                        AssetPathCacheList_.Add(Asset.GetInstanceID(), Uri.AssetPath);
                    }
                }
                else
                {
                    LLogger.LWarning($"Can't Create Asset : {Uri}");
                }
            }

            return Asset;
        }

        public void CreatePrefabAsync(AssetUri Uri, Action<UnityEngine.GameObject> Callback = null)
        {
            LoadAssetAsync<UnityEngine.GameObject>(AssetCacheType.Prefab, Uri.AssetPath, (IsLoaded) =>
            {
                if (!IsLoaded)
                {
                    Callback?.Invoke(null);
                    return;
                }

                Callback?.Invoke(CreatePrefabSync(Uri));
            });
        }

        public UnityEngine.GameObject CreatePrefabSync(AssetUri Uri)
        {
            return CreateAssetSync<UnityEngine.GameObject>(Uri);
        }

        public void CreateDataAsync(AssetUri Uri, Action<byte[]> Callback = null)
        {
            LoadAssetAsync<UnityEngine.TextAsset>(AssetCacheType.Data, Uri.AssetPath, (IsLoaded) =>
            {
                if (!IsLoaded)
                {
                    Callback?.Invoke(null);
                    return;
                }

                Callback?.Invoke(CreateDataSync(Uri));
            });
        }

        public byte[] CreateDataSync(AssetUri Uri)
        {
            var Asset = CreateAssetSync<UnityEngine.TextAsset>(Uri);
#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
            return (Asset as AssetInternalLoader.AssetInternalTextAsset)?.InternalBytes;
#endif
            return Asset?.bytes;
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