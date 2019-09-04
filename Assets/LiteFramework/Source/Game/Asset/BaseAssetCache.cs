using System.Collections;
using System.Collections.Generic;
using LiteFramework.Core.Log;

namespace LiteFramework.Game.Asset
{
    internal abstract class BaseAssetCache
    {
        public AssetCacheType AssetType { get; }
        public string AssetPath { get; }

        public bool IsLoad { get; protected set; }
        public bool Unused => (RefCount_ <= 0 && IsLoad == true);

        protected int RefCount_;
        protected readonly List<BaseAssetCache> DependenciesCache_;

        protected BaseAssetCache(AssetCacheType AssetType, string AssetPath)
        {
            this.AssetType = AssetType;
            this.AssetPath = AssetPath;

            this.IsLoad = false;
            this.RefCount_ = 0;
            this.DependenciesCache_ = new List<BaseAssetCache>();
        }

        public override string ToString()
        {
            return $"{AssetPath} - {AssetType}";
        }

        public abstract string[] GetAllDependencies();

        public abstract IEnumerator LoadAsync();

        public abstract void LoadSync();

        public void Unload()
        {
            if (RefCount_ > 0)
            {
                LLogger.LWarning($"{AssetPath} : RefCount > 0");
            }

            OnUnload();
            foreach (var Cache in DependenciesCache_)
            {
                if (Cache.Unused)
                {
                    Cache.Unload();
                }
            }
            DependenciesCache_.Clear();

            IsLoad = false;
            RefCount_ = 0;
        }

        public abstract UnityEngine.Object CreateAsset(string AssetName);

        public abstract void DeleteAsset(UnityEngine.Object Asset);

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

        public void AddDependencyCache(BaseAssetCache Cache)
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

        protected abstract void OnLoad();

        protected abstract void OnUnload();
    }
}