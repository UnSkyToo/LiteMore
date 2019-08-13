using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Logic;
using LiteFramework.Game.Lua;
using LiteFramework.Game.UI;

namespace LiteFramework.Game
{
    public static class LiteGameManager
    {
        public static bool Startup()
        {
            LLogger.LInfo($"{nameof(AssetManager)} Startup");
            if (!AssetManager.Startup())
            {
                LLogger.LError($"{nameof(AssetManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(UIManager)} Startup");
            if (!UIManager.Startup())
            {
                LLogger.LError($"{nameof(UIManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(LogicManager)} Startup");
            if (!LogicManager.Startup())
            {
                LLogger.LError($"{nameof(LogicManager)} Startup Failed");
                return false;
            }

#if LITE_USE_LUA_MODULE
            LLogger.LInfo($"{nameof(LuaRuntime)} Startup");
            if (!LuaRuntime.Startup())
            {
                LLogger.LError($"{nameof(LuaRuntime)} Startup Failed");
                return false;
            }
#endif

            return true;
        }

        public static void Shutdown()
        {
#if LITE_USE_LUA_MODULE
            LuaRuntime.Shutdown();
#endif
            LogicManager.Shutdown();
            UIManager.Shutdown();
            AssetManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            AssetManager.Tick(DeltaTime);
            UIManager.Tick(DeltaTime);
            LogicManager.Tick(DeltaTime);
#if LITE_USE_LUA_MODULE
            LuaRuntime.Tick(DeltaTime);
#endif
        }
    }
}