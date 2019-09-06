using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace LiteMore
{
    public static class Configure
    {
        public const int TileOrder = 0;
        public const int GroundOrder = 10;
        public const int NpcBackOrder = 15;
        public const int NpcOrder = 20;
        public const int NpcFrontOrder = 25;
        public const int SkyOrder = 30;
        public const int TopOrder = 40;
        public const int UIOrder = 50;

        public static readonly Transform CanvasRoot = GameObject.Find("Canvas").transform;
        public static readonly Transform AudioRoot = CanvasRoot.Find("Audio").transform;
        public static readonly Transform UIRoot = CanvasRoot.Find("UI").transform;

#if UNITY_EDITOR
        public static readonly string CacheFilePath = $"{Application.dataPath}/cache.txt";
#else
        public static readonly string CacheFilePath = $"{Application.persistentDataPath}/cache.txt";
#endif

        public const int WindowWidth = 1280;
        public const int WindowHeight = 720;
        public const int WindowLeft = -640;
        public const int WindowRight = 640;
        public const int WindowTop = 320;
        public const int WindowBottom = -320;
        public static readonly Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);
        public const float EnterBackgroundMaxTime = 90.0f;
        public const float TipsHoldTime = 0.3f;

        public static readonly Vector2 CoreBasePosition = new Vector2(WindowRight - 262.0f / 2.0f, 0);
        public static readonly Vector2 CoreTopPosition = new Vector2(WindowRight - 262.0f / 2.0f, 233.0f / 2.0f - 20);



#if LITE_USE_LUA_MODULE
        [LuaCallCSharp]
        public static IEnumerable<Type> LuaCallCSharpList = new List<Type>()
        {
            typeof(LiteMore.Combat.NpcAttrIndex),
            typeof(LiteMore.Combat.Npc.BaseNpc),

            typeof(LiteMore.Player.PlayerDps),
            typeof(LiteMore.Player.PlayerManager),
        };

        [CSharpCallLua]
        public static IEnumerable<Type> CSharpCallLuaList = new List<Type>()
        {
        };

        [GCOptimize]
        public static IEnumerable<Type> OptimizeList = new List<Type>()
        {
            typeof(LiteMore.Combat.NpcAttrIndex),
        };
#endif
    }
}