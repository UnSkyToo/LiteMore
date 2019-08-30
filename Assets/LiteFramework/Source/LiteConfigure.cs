using System;
using System.Collections.Generic;
using LiteFramework.Game.UI;

namespace LiteFramework
{
    public static class LiteConfigure
    {
        public const string LiteFrameworkVersion = "19.8.15.1";
        public const float EnterBackgroundMaxTime = 90.0f;
        public const string AssetBundleManifestName = "StreamingAssets.lite";
        public const string StandaloneAssetsName = "StandaloneAssets";
        public const string CanvasNormalName = "Canvas-Normal";

        public static readonly Dictionary<Type, UIDescriptor> UIDescList = new Dictionary<Type, UIDescriptor>();
    }
}