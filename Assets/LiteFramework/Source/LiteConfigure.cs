using System;
using System.Collections.Generic;
using LiteFramework.Game.UI;
using UnityEngine;
using XLua;

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


#if LITE_USE_LUA_MODULE
        [LuaCallCSharp]
        public static IEnumerable<Type> LuaCallCSharpList = new List<Type>()
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

        [CSharpCallLua]
        public static IEnumerable<Type> CSharpCallLuaList = new List<Type>()
        {
            typeof(Action),
            typeof(Action<float>),
            typeof(Action<GameObject>),
            typeof(Action<GameObject, Vector2>),
            typeof(Action<LuaTable>),
            typeof(Action<LuaTable, LuaTable>),
            typeof(Action<LuaTable, float>),
            typeof(LiteFramework.Interface.Lua.ILuaMainEntity),
        };

        [GCOptimize]
        public static IEnumerable<Type> OptimizeList = new List<Type>()
        {
            typeof(LiteFramework.Game.UI.UIDepthMode),
            typeof(LiteFramework.Game.UI.UIEventType),
        };
#endif
    }
}