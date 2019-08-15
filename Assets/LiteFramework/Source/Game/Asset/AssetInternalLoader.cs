//#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET

using System;
using System.IO;
using System.Collections.Generic;
using LiteFramework.Helper;
using LiteFramework.Core.Log;
using LiteFramework.Core.ObjectPool;
using UnityEditor;

namespace LiteFramework.Game.Asset
{
    internal class AssetInternalLoader : IAssetLoader
    {
        private readonly Dictionary<string, AssetBundleCacheBase> AssetBundleCacheList_ = new Dictionary<string, AssetBundleCacheBase>();
        private readonly Dictionary<string, List<Action>> LoadAssetBundleCallbackList_ = new Dictionary<string, List<Action>>();
        private readonly Dictionary<int, string> AssetBundlePathCacheList_ = new Dictionary<int, string>();

        internal AssetInternalLoader()
        {
        }

        public bool Startup()
        {
            AssetBundleCacheList_.Clear();
            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            return true;
        }

        public void Shutdown()
        {
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

        private AssetBundleType GetAssetBundleTypeWithName(string BundlePath)
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

        private AssetBundleCacheBase CreateAssetBundleCache<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            AssetBundleCacheBase Cache = null;

            switch (BundleType)
            {
                case AssetBundleType.Asset:
                    Cache = new AssetBundleCache<UnityEngine.Object>(BundleType, BundlePath);
                    break;
                case AssetBundleType.Prefab:
                    Cache = new PrefabBundleCache(BundleType, BundlePath);
                    break;
                case AssetBundleType.Data:
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

        private AssetBundleCacheBase LoadAssetBundleSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                return AssetBundleCacheList_[BundlePath];
            }
            return LoadAssetBundleCompletedSync<T>(BundleType, BundlePath);
        }

        private AssetBundleCacheBase LoadAssetBundleCompletedSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
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

        private void LoadAssetBundleCacheDependenciesSync<T>(AssetBundleCacheBase Cache, Action Callback = null) where T : UnityEngine.Object
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
            Callback?.Invoke(CreateAssetSync<T>(BundlePath, AssetName));
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
            Callback?.Invoke(CreatePrefabSync(BundlePath, AssetName));
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
            Callback?.Invoke(CreateDataSync(BundlePath, AssetName));
        }

        public void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateDataAsync(BundlePath, AssetName, Callback);
        }

        public byte[] CreateDataSync(string BundlePath, string AssetName)
        {
            byte[] Asset = null;
            BundlePath = BundlePath.ToLower();

            DataBundleCache DataCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                DataCache = LoadAssetBundleSync<UnityEngine.TextAsset>(AssetBundleType.Data, BundlePath) as DataBundleCache;
            }
            else
            {
                DataCache = AssetBundleCacheList_[BundlePath] as DataBundleCache;
            }

            if (DataCache != null)
            {
                Asset = DataCache.Buffer;
            }

            return Asset;
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

        public void DeleteUnusedAssetBundle()
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
            public AssetBundleType BundleType { get; }
            public string BundlePath { get; }
            public string BundleName => PathHelper.GetFileNameWithoutExt(BundlePath);

            public bool IsLoad { get; protected set; }
            public bool Unused => (RefCount_ <= 0 && IsLoad == true);

            protected int RefCount_;
            protected readonly List<AssetBundleCacheBase> DependenciesCache_;

            protected AssetBundleCacheBase(AssetBundleType BundleType, string BundlePath)
            {
                this.BundleType = BundleType;
                this.BundlePath = BundlePath;

                this.IsLoad = false;
                this.RefCount_ = 0;
                this.DependenciesCache_ = new List<AssetBundleCacheBase>();
            }

            protected virtual string GetInternalAssetPath()
            {
                return $"Assets/{LiteConfigure.StandaloneAssetsName}/{BundlePath}";
            }

            public virtual void LoadSync()
            {
                IsLoad = false;
                var FullPath = GetInternalAssetPath();
                var AssetList = AssetDatabase.LoadAllAssetsAtPath(FullPath);
                //Logger.DInfo($"Load AssetBundle : {FullPath}");

                if (AssetList.Length == 0)
                {
                    LLogger.LWarning($"Load AssetBundle : {FullPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    OnLoad(AssetList);
                }
            }

            public void Unload()
            {
                foreach (var Cache in DependenciesCache_)
                {
                    if (Cache.Unused)
                    {
                        Cache.Unload();
                    }
                }

                OnUnload();
                IsLoad = false;
                RefCount_ = 0;
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
                var Deps = new List<string>();
                Deps.AddRange(AssetDatabase.GetDependencies(GetInternalAssetPath()));

                for (var Index = 0; Index < Deps.Count; ++Index)
                {
                    Deps[Index] = Deps[Index].Substring($"Assets/{LiteConfigure.StandaloneAssetsName}/".Length).ToLower();
                }

                Deps.Remove(BundlePath);

                return Deps.ToArray();
            }

            public void AddDependencyCache(AssetBundleCacheBase Cache)
            {
                if (!DependenciesCache_.Contains(Cache))
                {
                    DependenciesCache_.Add(Cache);
                }
            }

            protected virtual void OnLoad(UnityEngine.Object[] AssetList)
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

            public AssetBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                AssetList_ = new Dictionary<string, T>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad(UnityEngine.Object[] Assets)
            {
                if (Assets != null)
                {
                    foreach (var Asset in Assets)
                    {
                        var Name = $"{Asset.name.ToLower()}_{Asset.GetType().Name.ToLower()}";
                        if (AssetList_.ContainsKey(Name))
                        {
                            LLogger.LWarning($"Repeat Asset : {Name}");
                            continue;
                        }

                        AssetList_.Add(Name, Asset as T);
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

            public PrefabBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
            }

            protected override void OnLoad(UnityEngine.Object[] AssetList)
            {
                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        if (Asset.name.ToLower() == BundleName.ToLower() && Asset is UnityEngine.GameObject ObjAsset)
                        {
                            var Pool = ObjectPoolManager.AddPool($"{BundlePath}_{Asset.name}".ToLower(), ObjAsset);
                            ObjectPools_.Add(Pool.PoolName, Pool);
                        }
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
            public byte[] Buffer { get; private set; }

            public DataBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                Buffer = null;
            }

            public override void LoadSync()
            {
                IsLoad = false;
                var FullPath = GetInternalAssetPath();

                if (!File.Exists(FullPath))
                {
                    LLogger.LWarning($"Load AssetBundle : {FullPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Buffer = File.ReadAllBytes(FullPath);
                }
            }

            protected override void OnUnload()
            {
                Buffer = null;
            }
        }
    }
}

//#endif