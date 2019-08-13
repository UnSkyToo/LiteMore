using System;
using System.Collections.Generic;
using LiteFramework.Game.UI;

namespace LiteFramework
{
    public static class LiteConfigure
    {
        public const float EnterBackgroundMaxTime = 90.0f;
        public const string AssetBundleManifestName = "StreamingAssets";
        //public const string StandaloneAssetsName = "StandaloneAssets";
        public const string StandaloneAssetsName = "Resources";

        public const string CanvasNormalName = "Canvas-Normal";
        // use lua module
        // #define LITE_USE_LUA_MODULE
        public const string LuaEntryFileName = "main";

        public static readonly Dictionary<Type, UIDescriptor> UIDescList = new Dictionary<Type, UIDescriptor>();
    }
}