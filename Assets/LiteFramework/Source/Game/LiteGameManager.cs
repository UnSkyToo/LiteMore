using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Lua;
using LiteFramework.Game.UI;
using LiteFramework.Interface;

namespace LiteFramework.Game
{
    public static class LiteGameManager
    {
        public static ILogic MainLogic { get; private set; }

        public static bool Startup(ILogic Logic)
        {
            MainLogic = Logic;

            LLogger.LInfo($"{nameof(AssetManager)} Startup");
            if (!AssetManager.Startup())
            {
                LLogger.LError($"{nameof(AssetManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(AudioManager)} Startup");
            if (!AudioManager.Startup())
            {
                LLogger.LError($"{nameof(AudioManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(UIManager)} Startup");
            if (!UIManager.Startup())
            {
                LLogger.LError($"{nameof(UIManager)} Startup Failed");
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

            if (MainLogic == null || !MainLogic.Startup())
            {
                LLogger.LError($"Logic Startup Failed");
                return false;
            }

            return true;
        }

        public static void Shutdown()
        {
            MainLogic?.Shutdown();
#if LITE_USE_LUA_MODULE
            LuaRuntime.Shutdown();
#endif
            UIManager.Shutdown();
            AudioManager.Shutdown();
            AssetManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            AssetManager.Tick(DeltaTime);
            AudioManager.Tick(DeltaTime);
            UIManager.Tick(DeltaTime);
#if LITE_USE_LUA_MODULE
            LuaRuntime.Tick(DeltaTime);
#endif
            MainLogic?.Tick(DeltaTime);
        }
    }
}