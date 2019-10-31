#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LiteFramework.Helper;
using LiteFramework.Core.Log;
using UnityEditor;

namespace LiteFramework.Game.Asset
{
    internal class AssetInternalLoader : BaseAssetLoader
    {
        internal AssetInternalLoader()
            : base()
        {
        }

        protected override BaseAssetCache CreateAssetCache<T>(AssetCacheType AssetType, string AssetPath)
        {
            BaseAssetCache Cache = null;

            switch (AssetType)
            {
                case AssetCacheType.Asset:
                    Cache = new AssetInternalCache<UnityEngine.Object>(AssetType, AssetPath);
                    break;
                case AssetCacheType.Prefab:
                    Cache = new PrefabAssetInternalCache(AssetType, AssetPath);
                    break;
                case AssetCacheType.Data:
                    Cache = new DataAssetInternalCache(AssetType, AssetPath);
                    break;
                default:
                    break;
            }

            return Cache;
        }

        private class AssetInternal
        {
            public string AssetPath { get; }

            public AssetInternal(string Path)
            {
                AssetPath = Path;
            }

            private string GetInternalAssetPath()
            {
                return $"Assets/{LiteConfigure.StandaloneAssetsName}/{AssetPath}";
            }

            public T[] LoadAllAssets<T>()
            {
                var FullPath = GetInternalAssetPath();
                var AssetList = AssetDatabase.LoadAllAssetsAtPath(FullPath);
                var Result = new List<T>();
                foreach (var Asset in AssetList)
                {
                    if (Asset is T Obj)
                    {
                        Result.Add(Obj);
                    }
                }

                return Result.ToArray();
            }
        }

        private class AssetInternalCreateRequest : IEnumerator
        {
            public object Current => assetBundle;
            public bool isDone { get; private set; }
            public AssetInternal assetBundle { get; private set; }

            internal AssetInternalCreateRequest(string Path)
            {
                assetBundle = new AssetInternal(Path);
                isDone = true;
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }
        }

        internal class AssetInternalTextAsset : UnityEngine.TextAsset
        {
            public byte[] InternalBytes { get; }

            public AssetInternalTextAsset(byte[] Buffer)
                : base("Internal Text Asset")
            {
                this.InternalBytes = Buffer;
            }
        }

        private abstract class BaseAssetInternalCache : BaseAssetCache
        {
            public string AssetName => PathHelper.GetFileNameWithoutExt(AssetPath);
            public AssetInternal Bundle { get; private set; }

            protected BaseAssetInternalCache(AssetCacheType AssetType, string AssetPath)
                : base(AssetType, AssetPath)
            {
                this.Bundle = null;
            }

            protected virtual string GetInternalAssetPath()
            {
                return $"Assets/{LiteConfigure.StandaloneAssetsName}/{AssetPath}";
            }

            private AssetInternalCreateRequest CreateInternalRequestAsync(string Path)
            {
                return new AssetInternalCreateRequest(Path);
            }

            private AssetInternal CreateInternalRequestSync(string Path)
            {
                return new AssetInternal(Path);
            }

            public override string[] GetAllDependencies()
            {
                var Deps = new List<string>();
                Deps.AddRange(AssetDatabase.GetDependencies(GetInternalAssetPath()));

                for (var Index = 0; Index < Deps.Count; ++Index)
                {
                    Deps[Index] = Deps[Index].Substring($"Assets/{LiteConfigure.StandaloneAssetsName}/".Length).ToLower();
                }

                Deps.Remove(AssetPath);

                return Deps.ToArray();
            }

            public override IEnumerator LoadAsync()
            {
                IsLoad = false;
                var Request = CreateInternalRequestAsync(AssetPath);
                yield return Request;

                if (!Request.isDone)
                {
                    LLogger.LWarning($"Load AssetInternal : {AssetPath} Failed");
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

            public override void LoadSync()
            {
                IsLoad = false;
                var Request = CreateInternalRequestSync(AssetPath);

                if (Request == null)
                {
                    LLogger.LWarning($"Load AssetInternal : {AssetPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle = Request;
                    OnLoad();
                }
            }
        }

        private class AssetInternalCache<T> : BaseAssetInternalCache where T : UnityEngine.Object
        {
            private readonly Dictionary<string, T> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public AssetInternalCache(AssetCacheType BundleType, string AssetPath)
                : base(BundleType, AssetPath)
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

        private class PrefabAssetInternalCache : BaseAssetInternalCache
        {
            private readonly Dictionary<string, UnityEngine.GameObject> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public PrefabAssetInternalCache(AssetCacheType BundleType, string AssetPath)
                : base(BundleType, AssetPath)
            {
                AssetList_ = new Dictionary<string, UnityEngine.GameObject>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<UnityEngine.GameObject>();

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
                    /*foreach (var Asset in AssetList_)
                    {
                        UnityEngine.Resources.UnloadAsset(Asset.Value);
                    }*/
                    AssetList_.Clear();
                }

                AssetInstanceIDList_.Clear();
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

        private class DataAssetInternalCache : BaseAssetInternalCache
        {
            public byte[] Buffer { get; private set; }
            private AssetInternalTextAsset Asset_;

            public DataAssetInternalCache(AssetCacheType BundleType, string AssetPath)
                : base(BundleType, AssetPath)
            {
                Buffer = null;
                Asset_ = null;
            }

            protected override void OnLoad()
            {
                IsLoad = false;
                var FullPath = GetInternalAssetPath();

                if (!File.Exists(FullPath))
                {
                    LLogger.LWarning($"Load AssetInternal : {FullPath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Buffer = File.ReadAllBytes(FullPath);
                    Asset_ = new AssetInternalTextAsset(Buffer);
                }
            }

            protected override void OnUnload()
            {
                Buffer = null;
            }

            public override UnityEngine.Object CreateAsset(string AssetName)
            {
                return Asset_;
            }

            public override void DeleteAsset(UnityEngine.Object Asset)
            {
            }
        }
    }
}

#endif