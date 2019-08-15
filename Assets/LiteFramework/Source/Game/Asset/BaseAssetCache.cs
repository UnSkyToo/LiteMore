using System.Collections.Generic;
using LiteFramework.Core.Log;
using LiteFramework.Core.ObjectPool;

namespace LiteFramework.Game.Asset
{
    internal abstract class BaseAssetCache
    {
        public AssetBundleType BundleType { get; }
        public string BundlePath { get; }

        public bool IsLoad { get; private set; }
        public bool Unused => (RefCount_ <= 0 && IsLoad == true);

        private int RefCount_;
        protected readonly List<BaseAssetCache> DependenciesCache_;

        protected BaseAssetCache(AssetBundleType BundleType, string BundlePath)
        {
            this.BundleType = BundleType;
            this.BundlePath = BundlePath;

            this.IsLoad = false;
            this.RefCount_ = 0;
            this.DependenciesCache_ = new List<BaseAssetCache>();
        }

        public virtual void Unload()
        {
            if (RefCount_ > 0)
            {
                LLogger.LWarning($"{BundlePath} : RefCount > 0");
            }

            OnUnload();
            IsLoad = false;
            RefCount_ = 0;
            DependenciesCache_.Clear();
        }

        protected virtual void IncRef()
        {
            RefCount_++;
            foreach (var Cache in DependenciesCache_)
            {
                Cache.IncRef();
            }
        }

        protected virtual void DecRef()
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

        protected abstract void OnLoad();
        protected abstract void OnUnload();
    }

    internal class NormalAssetCache<T> : BaseAssetCache where T : UnityEngine.Object
    {
        private readonly Dictionary<string, T> AssetList_ = null;
        private readonly List<int> AssetInstanceIDList_ = null;

        public NormalAssetCache(AssetBundleType BundleType, string BundlePath)
            : base(BundleType, BundlePath)
        {
            AssetList_ = new Dictionary<string, T>();
            AssetInstanceIDList_ = new List<int>();
        }

        protected override void OnLoad()
        {
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

    internal class PrefabAssetCache : BaseAssetCache
    {
        private readonly Dictionary<string, ObjectPoolEntity> ObjectPools_ = new Dictionary<string, ObjectPoolEntity>();
        private readonly Dictionary<int, string> GameObjectPoolNames_ = new Dictionary<int, string>();

        public PrefabAssetCache(AssetBundleType BundleType, string BundlePath)
            : base(BundleType, BundlePath)
        {
        }

        protected override void OnLoad()
        {
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

    internal class DataAssetCache : BaseAssetCache
    {
        private readonly Dictionary<string, UnityEngine.TextAsset> AssetList_ = null;
        private readonly List<int> AssetInstanceIDList_ = null;

        public DataAssetCache(AssetBundleType BundleType, string BundlePath)
            : base(BundleType, BundlePath)
        {
            AssetList_ = new Dictionary<string, UnityEngine.TextAsset>();
            AssetInstanceIDList_ = new List<int>();
        }

        protected override void OnLoad()
        {
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