﻿using System.Collections.Generic;

namespace LiteFramework
{
    public static class LiteConfigure
    {
        public const string LiteFrameworkVersion = "19.8.15.1";
        public const float EnterBackgroundMaxTime = 90.0f;
        public const string AssetBundleManifestName = "StreamingAssets.lite";
        public const string StandaloneAssetsName = "StandaloneAssets";
        public const string CanvasNormalName = "Canvas-Normal";

        public static readonly Dictionary<System.Type, LiteFramework.Game.UI.UIDescriptor> UIDescList = new Dictionary<System.Type, LiteFramework.Game.UI.UIDescriptor>();


#if LITE_USE_LUA_MODULE
        [XLua.LuaCallCSharp]
        public static IEnumerable<System.Type> LuaCallCSharpList = new List<System.Type>()
        {
            typeof(LiteFramework.Game.UI.BaseUI),
            typeof(LiteFramework.Game.UI.UIDepthMode),
            typeof(LiteFramework.Game.UI.UIEventType),
            typeof(LiteFramework.Game.Lua.LuaRuntime),
            typeof(LiteFramework.Helper.UIHelper),
            // Motions
            typeof(LiteFramework.Core.Motion.BaseMotion),
            typeof(LiteFramework.Core.Motion.CallbackMotion),
            typeof(LiteFramework.Core.Motion.FadeMotion),
            typeof(LiteFramework.Core.Motion.FadeInMotion),
            typeof(LiteFramework.Core.Motion.FadeOutMotion),
            typeof(LiteFramework.Core.Motion.MoveMotion),
            typeof(LiteFramework.Core.Motion.ParallelMotion),
            typeof(LiteFramework.Core.Motion.RepeatSequenceMotion),
            typeof(LiteFramework.Core.Motion.ScaleMotion),
            typeof(LiteFramework.Core.Motion.SequenceMotion),
            typeof(LiteFramework.Core.Motion.WaitTimeMotion),
            typeof(LiteFramework.Core.Motion.MotionManager),
        };

        [XLua.CSharpCallLua]
        public static IEnumerable<System.Type> CSharpCallLuaList = new List<System.Type>()
        {
            typeof(System.Action),
            typeof(System.Action<float>),
            typeof(System.Action<UnityEngine.GameObject>),
            typeof(System.Action<UnityEngine.GameObject, UnityEngine.Vector2>),
            typeof(System.Action<XLua.LuaTable>),
            typeof(System.Action<XLua.LuaTable, XLua.LuaTable>),
            typeof(System.Action<XLua.LuaTable, float>),
            typeof(LiteFramework.Interface.Lua.ILuaMainEntity),
        };

        [XLua.GCOptimize]
        public static IEnumerable<System.Type> OptimizeList = new List<System.Type>()
        {
            typeof(LiteFramework.Game.UI.UIDepthMode),
            typeof(LiteFramework.Game.UI.UIEventType),
        };
#endif
    }
}