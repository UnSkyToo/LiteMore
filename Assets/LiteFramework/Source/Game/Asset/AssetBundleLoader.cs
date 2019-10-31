using System;
using System.Collections;
using System.Collections.Generic;
using LiteFramework.Helper;
using LiteFramework.Core.Log;

namespace LiteFramework.Game.Asset
{
    internal sealed class AssetBundleLoader : BaseAssetLoader
    {
        private UnityEngine.AssetBundleManifest Manifest_ = null;
        private readonly List<string> AssetBundlePathList_ = new List<string>();

        internal AssetBundleLoader()
            : base()
        {
        }

        public override bool Startup()
        {
            if (!base.Startup())
            {
                return false;
            }

            Manifest_ = null;
            AssetBundlePathList_.Clear();

            if (!LoadAssetBundleManifest(LiteConfigure.AssetBundleManifestName))
            {
                return false;
            }

            return true;
        }

        public override void Shutdown()
        {
            if (Manifest_ != null)
            {
                UnityEngine.Resources.UnloadAsset(Manifest_);
                Manifest_ = null;
            }

            AssetBundlePathList_.Clear();

            UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
            base.Shutdown();
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

        protected override BaseAssetCache CreateAssetCache<T>(AssetCacheType AssetType, string AssetPath)
        {
            BaseAssetCache Cache = null;

            switch (AssetType)
            {
                case AssetCacheType.Asset:
                    Cache = new AssetBundleCache<UnityEngine.Object>(AssetType, AssetPath, Manifest_);
                    break;
                case AssetCacheType.Prefab:
                    Cache = new PrefabAssetBundleCache(AssetType, AssetPath, Manifest_);
                    break;
                case AssetCacheType.Data:
                    Cache = new DataAssetBundleCache(AssetType, AssetPath, Manifest_);
                    break;
                default:
                    break;
            }

            return Cache;
        }

        protected override void LoadAssetAsync<T>(AssetCacheType AssetType, string AssetPath, Action<bool> Callback = null)
        {
            AssetPath = AssetPath.ToLower();
            if (!AssetBundlePathList_.Contains(AssetPath))
            {
                Callback?.Invoke(false);
                return;
            }

            base.LoadAssetAsync<T>(AssetType, AssetPath, Callback);
        }

        protected override BaseAssetCache LoadAssetSync<T>(AssetCacheType AssetType, string AssetPath)
        {
            AssetPath = AssetPath.ToLower();
            if (!AssetBundlePathList_.Contains(AssetPath))
            {
                return null;
            }

            return base.LoadAssetSync<T>(AssetType, AssetPath);
        }

        private abstract class BaseAssetBundleCache : BaseAssetCache
        {
            protected readonly UnityEngine.AssetBundleManifest Manifest_;
            protected UnityEngine.AssetBundle Bundle_;

            protected BaseAssetBundleCache(AssetCacheType AssetType, string AssetPath, UnityEngine.AssetBundleManifest Manifest)
                : base(AssetType, AssetPath)
            {
                this.Manifest_ = Manifest;
                this.Bundle_ = null;
            }

            private UnityEngine.AssetBundleCreateRequest CreateBundleRequestAsync(string Path)
            {
                var FullPath = PathHelper.GetAssetFullPath(Path);
                if (string.IsNullOrEmpty(FullPath))
                {
                    return null;
                }
                return UnityEngine.AssetBundle.LoadFromFileAsync(FullPath);
            }

            private UnityEngine.AssetBundle CreateBundleRequestSync(string Path)
            {
                var FullPath = PathHelper.GetAssetFullPath(Path);
                if (string.IsNullOrEmpty(FullPath))
                {
                    return null;
                }
                return UnityEngine.AssetBundle.LoadFromFile(FullPath);
            }

            public override string[] GetAllDependencies()
            {
                return this.Manifest_.GetAllDependencies(AssetPath);
            }

            public override IEnumerator LoadAsync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestAsync(AssetPath);
                yield return Request;

                if (!Request.isDone)
                {
                    LLogger.LWarning($"Load AssetBundle : {AssetPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle_ = Request.assetBundle;
                    OnLoad();
                }

                yield break;
            }

            public override void LoadSync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestSync(AssetPath);

                if (!Request)
                {
                    LLogger.LWarning($"Load AssetBundle : {AssetPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle_ = Request;
                    OnLoad();
                }
            }

            protected override void OnUnload()
            {
                if (Bundle_ != null)
                {
                    Bundle_.Unload(true);
                    Bundle_ = null;
                }
            }
        }

        private sealed class AssetBundleCache<T> : BaseAssetBundleCache where T : UnityEngine.Object
        {
            private readonly Dictionary<string, T> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public AssetBundleCache(AssetCacheType AssetType, string AssetPath, UnityEngine.AssetBundleManifest Manifest)
                : base(AssetType, AssetPath, Manifest)
            {
                AssetList_ = new Dictionary<string, T>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle_.LoadAllAssets<T>();
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
                base.OnUnload();
            }

            public override UnityEngine.Object CreateAsset(string AssetName)
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

            public override void DeleteAsset(UnityEngine.Object Asset)
            {
                /*if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    DecRef();
                }*/
            }
        }

        private sealed class PrefabAssetBundleCache : BaseAssetBundleCache
        {
            private readonly Dictionary<string, UnityEngine.GameObject> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public PrefabAssetBundleCache(AssetCacheType AssetType, string AssetPath, UnityEngine.AssetBundleManifest Manifest)
                : base(AssetType, AssetPath, Manifest)
            {
                AssetList_ = new Dictionary<string, UnityEngine.GameObject>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle_.LoadAllAssets<UnityEngine.GameObject>();

                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        var Name = Asset.name.ToLower();
                        if (AssetList_.ContainsKey(Name))
                        {
                            LLogger.LWarning($"Repeat Asset : {Name}");
                            continue;
                        }
                        AssetList_.Add(Name, Asset);
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
                base.OnUnload();
            }

            public override UnityEngine.Object CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                var Obj = UnityEngine.Object.Instantiate(AssetList_[AssetName]);
                AssetInstanceIDList_.Add(Obj.GetInstanceID());
                IncRef();
                return Obj;
            }

            public override void DeleteAsset(UnityEngine.Object Asset)
            {
                if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    AssetInstanceIDList_.Remove(Asset.GetInstanceID());
                    UnityEngine.Object.Destroy(Asset);
                    DecRef();
                }
            }
        }

        private sealed class DataAssetBundleCache : BaseAssetBundleCache
        {
            private readonly Dictionary<string, UnityEngine.TextAsset> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public DataAssetBundleCache(AssetCacheType AssetType, string AssetPath, UnityEngine.AssetBundleManifest Manifest)
                : base(AssetType, AssetPath, Manifest)
            {
                AssetList_ = new Dictionary<string, UnityEngine.TextAsset>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle_.LoadAllAssets<UnityEngine.TextAsset>();
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
                base.OnUnload();
            }

            public override UnityEngine.Object CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                //IncRef();
                return AssetList_[AssetName];
            }

            public override void DeleteAsset(UnityEngine.Object Asset)
            {
                /*if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    DecRef();
                }*/
            }
        }
    }
}