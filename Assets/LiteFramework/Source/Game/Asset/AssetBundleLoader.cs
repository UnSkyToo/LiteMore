using System;
using System.Collections;
using System.Collections.Generic;
using LiteFramework.Helper;
using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Log;
using LiteFramework.Core.ObjectPool;

namespace LiteFramework.Game.Asset
{
    internal class AssetBundleLoader: IAssetLoader
    {
        private static UnityEngine.AssetBundleManifest Manifest_ = null;
        private readonly List<string> AssetBundlePathList_ = new List<string>();
        private readonly Dictionary<string, AssetBundleCacheBase> AssetBundleCacheList_ = new Dictionary<string, AssetBundleCacheBase>();
        private readonly Dictionary<string, List<Action>> LoadAssetBundleCallbackList_ = new Dictionary<string, List<Action>>();
        private readonly Dictionary<int, string> AssetBundlePathCacheList_ = new Dictionary<int, string>();

        internal AssetBundleLoader()
        {
        }

        public bool Startup()
        {
            Manifest_ = null;
            AssetBundlePathList_.Clear();
            AssetBundleCacheList_.Clear();
            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            if (!LoadAssetBundleManifest(LiteConfigure.AssetBundleManifestName))
            {
                return false;
            }
            
            return true;
        }

        public void Shutdown()
        {
            if (Manifest_ != null)
            {
                UnityEngine.Resources.UnloadAsset(Manifest_);
                Manifest_ = null;
            }

            AssetBundlePathList_.Clear();

            if (AssetBundleCacheList_.Count > 0)
            {
                foreach (var Cache in AssetBundleCacheList_)
                {
                    Cache.Value.Unload();
                }
                AssetBundleCacheList_.Clear();
            }

            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            UnityEngine.Resources.UnloadUnusedAssets();
            UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Tick(float DeltaTime)
        {
        }

        private static UnityEngine.AssetBundleCreateRequest CreateBundleRequestAsync(string Path)
        {
            var FullPath = PathHelper.GetAssetFullPath(Path);
            if (string.IsNullOrEmpty(FullPath))
            {
                return null;
            }
            return UnityEngine.AssetBundle.LoadFromFileAsync(FullPath);
        }

        private static UnityEngine.AssetBundle CreateBundleRequestSync(string Path)
        {
            var FullPath = PathHelper.GetAssetFullPath(Path);
            if (string.IsNullOrEmpty(FullPath))
            {
                return null;
            }
            return UnityEngine.AssetBundle.LoadFromFile(FullPath);
        }

        private AssetCacheType GetAssetBundleTypeWithName(string BundlePath)
        {
            var Ext = PathHelper.GetFileExt(BundlePath);

            switch (Ext)
            {
                case ".prefab":
                    return AssetCacheType.Prefab;
                case ".bytes":
                    return AssetCacheType.Data;
                default:
                    return AssetCacheType.Asset;
            }
        }

        private bool LoadAssetBundleManifest(string ResPath)
        {
            var FullPath = PathHelper.GetAssetFullPath(ResPath);
            if (string.IsNullOrEmpty(FullPath))
            {
                LLogger.LError($"LoadAssetBundleManifest Failed : {FullPath}");
                return false;
            }

            var Bundle = UnityEngine.AssetBundle.LoadFromFile(FullPath);
            if (Bundle != null)
            {
                Manifest_ = Bundle.LoadAsset<UnityEngine.AssetBundleManifest>("AssetBundleManifest");
                Bundle.Unload(false);
                AssetBundlePathList_.AddRange(Manifest_.GetAllAssetBundles());
                return true;
            }
            else
            {
                LLogger.LError($"LoadAssetBundleManifest Failed : {FullPath}");
            }

            return false;
        }

        private AssetBundleCacheBase CreateAssetBundleCache<T>(AssetCacheType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            AssetBundleCacheBase Cache = null;

            switch (BundleType)
            {
                case AssetCacheType.Asset:
                    /*var RealType = GetAssetBundleTypeWithName(BundlePath);

                    switch (RealType)
                    {
                        case AssetBundleType.Asset:
                            Cache = new AssetBundleCache<T>(BundleType, BundlePath);
                            break;
                        case AssetBundleType.Prefab:
                            Cache = new PrefabBundleCache(BundleType, BundlePath);
                            break;
                        case AssetBundleType.Data:
                            Cache = new DataBundleCache(BundleType, BundlePath);
                            break;
                        default:
                            break;
                    }*/
                    Cache = new AssetBundleCache<UnityEngine.Object>(BundleType, BundlePath);
                    break;
                case AssetCacheType.Prefab:
                    Cache = new PrefabBundleCache(BundleType, BundlePath);
                    break;
                case AssetCacheType.Data:
                    Cache = new DataBundleCache(BundleType, BundlePath);
                    break;
                default:
                    break;
            }

            return Cache;
        }

        public bool AssetBundleCacheExisted(string BundlePath)
        {
            BundlePath = BundlePath.ToLower();

            if (AssetBundleCacheList_.ContainsKey(BundlePath) && AssetBundleCacheList_[BundlePath].IsLoad)
            {
                return true;
            }

            return false;
        }

        private bool LoadAssetBundleAsync<T>(AssetCacheType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                Callback?.Invoke();
                return true;
            }

            if (!AssetBundlePathList_.Contains(BundlePath))
            {
                Callback?.Invoke();
                return false;
            }

            if (!LoadAssetBundleCallbackList_.ContainsKey(BundlePath))
            {
                LoadAssetBundleCallbackList_.Add(BundlePath, new List<Action> {Callback});

                return LoadAssetBundleCacheCompletedAsync<T>(BundleType, BundlePath, () =>
                {
                    foreach (var LoadCallback in LoadAssetBundleCallbackList_[BundlePath])
                    {
                        LoadCallback?.Invoke();
                    }
                    LoadAssetBundleCallbackList_.Remove(BundlePath);
                });
            }
            else
            {
                LoadAssetBundleCallbackList_[BundlePath].Add(Callback);
            }

            return true;
        }

        private AssetBundleCacheBase LoadAssetBundleSync<T>(AssetCacheType BundleType, string BundlePath) where T : UnityEngine.Object
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

        private bool LoadAssetBundleCacheCompletedAsync<T>(AssetCacheType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
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

        private AssetBundleCacheBase LoadAssetBundleCacheCompletedSync<T>(AssetCacheType BundleType, string BundlePath) where T : UnityEngine.Object
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
            var IsLoaded = LoadAssetBundleAsync<T>(AssetCacheType.Asset, BundlePath, () =>
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
                AssetCache = LoadAssetBundleSync<UnityEngine.Object>(AssetCacheType.Asset, BundlePath) as AssetBundleCache<UnityEngine.Object>;
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
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.Object>(AssetCacheType.Prefab, BundlePath, () =>
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
                PrefabCache = LoadAssetBundleSync<UnityEngine.Object>(AssetCacheType.Prefab, BundlePath) as PrefabBundleCache;
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
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.TextAsset>(AssetCacheType.Data, BundlePath, () =>
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
                DataCache = LoadAssetBundleSync<UnityEngine.TextAsset>(AssetCacheType.Data, BundlePath) as DataBundleCache;
            }
            else
            {
                DataCache = AssetBundleCacheList_[BundlePath] as DataBundleCache;
            }

            if (DataCache != null)
            {
                Asset = DataCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
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

            if (AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var BundlePath = AssetBundlePathCacheList_[Asset.GetInstanceID()];
                AssetBundlePathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetBundleCacheExisted(BundlePath))
                {
                    if (typeof(T) != typeof(UnityEngine.GameObject))
                    {
                        if (AssetBundleCacheList_[BundlePath] is AssetBundleCache<T> Cache)
                        {
                            Cache.DeleteAsset(Asset);
                        }
                    }
                }
            }
        }

        public void DeleteAsset(UnityEngine.GameObject Asset)
        {
            if (Asset == null)
            {
                return;
            }

            if (AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var BundlePath = AssetBundlePathCacheList_[Asset.GetInstanceID()];
                AssetBundlePathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetBundleCacheExisted(BundlePath))
                {
                    if (AssetBundleCacheList_[BundlePath] is PrefabBundleCache Cache)
                    {
                        Cache.DeleteAsset(Asset);
                    }
                }
            }
        }

        public void DeleteUnusedAsset()
        {
            var RemoveList = new List<string>();

            foreach (var Cache in AssetBundleCacheList_)
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
                    AssetBundleCacheList_[Key].Unload();
                    AssetBundleCacheList_.Remove(Key);
                }

                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private class AssetBundleCacheBase
        {
            public AssetCacheType BundleType { get; }
            public string BundlePath { get; }
            public UnityEngine.AssetBundle Bundle { get; private set; }

            public bool IsLoad { get; private set; }
            public bool Unused => (RefCount_ <= 0 && IsLoad == true);

            private int RefCount_;
            protected readonly List<AssetBundleCacheBase> DependenciesCache_;

            protected AssetBundleCacheBase(AssetCacheType BundleType, string BundlePath)
            {
                this.BundleType = BundleType;
                this.BundlePath = BundlePath;
                this.Bundle = null;

                this.IsLoad = false;
                this.RefCount_ = 0;
                this.DependenciesCache_ = new List<AssetBundleCacheBase>();
            }

            public IEnumerator LoadAsync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestAsync(BundlePath);
                yield return Request;

                LLogger.LInfo($"Load AssetBundle : {BundlePath}");

                if (!Request.isDone)
                {
                    LLogger.LWarning($"Load AssetBundle : {BundlePath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle = Request.assetBundle;
                    OnLoad();
                }

                yield break;
            }

            public void LoadSync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestSync(BundlePath);

                if (!Request)
                {
                    LLogger.LWarning($"Load AssetBundle : {BundlePath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle = Request;
                    OnLoad();
                }
            }

            public void Unload()
            {
                if (RefCount_ > 0)
                {
                    LLogger.LWarning($"{BundlePath} : RefCount > 0");
                }

                OnUnload();
                UnloadAssetBundle();
                IsLoad = false;
                RefCount_ = 0;
                DependenciesCache_.Clear();
            }

            private void UnloadAssetBundle()
            {
                if (Bundle != null)
                {
                    Bundle.Unload(true);
                    Bundle = null;
                }

                foreach (var Cache in DependenciesCache_)
                {
                    if (Cache.Unused)
                    {
                        Cache.Unload();
                    }
                }
            }

            protected void IncRef()
            {
                RefCount_++;
                foreach (var Cache in DependenciesCache_)
                {
                    Cache.IncRef();
                }
            }

            protected void DecRef()
            {
                if (RefCount_ > 0)
                {
                    RefCount_--;
                    foreach (var Cache in DependenciesCache_)
                    {
                        Cache.DecRef();
                    }
                }
            }

            public virtual string[] GetAllDependencies()
            {
                return Manifest_.GetAllDependencies(BundlePath);
            }

            public void AddDependencyCache(AssetBundleCacheBase Cache)
            {
                if (Cache == null)
                {
                    return;
                }

                if (!DependenciesCache_.Contains(Cache))
                {
                    DependenciesCache_.Add(Cache);
                }
            }

            protected virtual void OnLoad()
            {
            }

            protected virtual void OnUnload()
            {
            }
        }

        private class AssetBundleCache<T> : AssetBundleCacheBase where T : UnityEngine.Object
        {
            private readonly Dictionary<string, T> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public AssetBundleCache(AssetCacheType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                AssetList_ = new Dictionary<string, T>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<T>();
                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        var Name = $"{Asset.name.ToLower()}_{Asset.GetType().Name.ToLower()}";
                        if (AssetList_.ContainsKey(Name))
                        {
                            LLogger.LWarning($"Repeat Asset : {Name}");
                            continue;
                        }
                        AssetList_.Add(Name, Asset);
                        AssetInstanceIDList_.Add(Asset.GetInstanceID());
                    }
                }
            }

            protected override void OnUnload()
            {
                if (AssetList_.Count > 0)
                {
                    foreach (var Asset in AssetList_)
                    {
                        UnityEngine.Resources.UnloadAsset(Asset.Value);
                    }
                    AssetList_.Clear();
                }

                AssetInstanceIDList_.Clear();
            }

            public T CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                //IncRef(); // asset don't inc ref (eg : sprite, audio. because there is no way to delete asset)
                // like : xxx.sprite = AssetManager.CreateAsset<Sprite>("xxx");
                // AssetManager.DeleteAsset(xxx); // delete obj, but sprite???
                // AssetManager.DeleteAsset(xxx.sprite); AssetManager.DeleteAsset(xxx); // bad way
                return AssetList_[AssetName];
            }

            public void DeleteAsset(T Asset)
            {
                if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    //DecRef();
                }
            }
        }

        private class PrefabBundleCache : AssetBundleCacheBase
        {
            private readonly Dictionary<string, ObjectPoolEntity> ObjectPools_ = new Dictionary<string, ObjectPoolEntity>();
            private readonly Dictionary<int, string> GameObjectPoolNames_ = new Dictionary<int, string>();

            public PrefabBundleCache(AssetCacheType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<UnityEngine.GameObject>();

                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        var Pool = ObjectPoolManager.AddPool($"{BundlePath}_{Asset.name}".ToLower(), Asset);
                        ObjectPools_.Add(Pool.PoolName, Pool);
                    }
                }
            }

            protected override void OnUnload()
            {
                if (ObjectPools_.Count > 0)
                {
                    foreach (var Pool in ObjectPools_)
                    {
                        ObjectPoolManager.DeletePool(Pool.Value);
                        UnityEngine.Object.DestroyImmediate(Pool.Value.Prefab, true);
                    }

                    ObjectPools_.Clear();
                }

                GameObjectPoolNames_.Clear();
            }

            public UnityEngine.GameObject CreateAsset(string AssetName)
            {
                var PoolName = $"{BundlePath}_{AssetName}".ToLower();
                if (!ObjectPools_.ContainsKey(PoolName))
                {
                    return null;
                }

                var Obj = ObjectPools_[PoolName].Spawn();
                if (!GameObjectPoolNames_.ContainsKey(Obj.GetInstanceID()))
                {
                    GameObjectPoolNames_.Add(Obj.GetInstanceID(), PoolName);
                }
                IncRef();
                return Obj;
            }

            public void DeleteAsset(UnityEngine.GameObject Asset)
            {
                if (GameObjectPoolNames_.ContainsKey(Asset.GetInstanceID()))
                {
                    var AssetName = GameObjectPoolNames_[Asset.GetInstanceID()];
                    ObjectPools_[AssetName].Recycle(Asset);
                    DecRef();
                }
            }
        }

        private class DataBundleCache : AssetBundleCacheBase
        {
            private readonly Dictionary<string, UnityEngine.TextAsset> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public DataBundleCache(AssetCacheType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                AssetList_ = new Dictionary<string, UnityEngine.TextAsset>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<UnityEngine.TextAsset>();
                if (AssetList != null && AssetList.Length > 0)
                {
                    foreach (var Asset in AssetList)
                    {
                        AssetList_.Add(Asset.name.ToLower(), Asset);
                        AssetInstanceIDList_.Add(Asset.GetInstanceID());
                    }
                }
            }

            protected override void OnUnload()
            {
                if (AssetList_.Count > 0)
                {
                    foreach (var Asset in AssetList_)
                    {
                        UnityEngine.Resources.UnloadAsset(Asset.Value);
                    }
                    AssetList_.Clear();
                }

                AssetInstanceIDList_.Clear();
            }

            public UnityEngine.TextAsset CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                IncRef();
                return AssetList_[AssetName];
            }

            public void DeleteAsset(UnityEngine.TextAsset Asset)
            {
                if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    DecRef();
                }
            }
        }
    }
}