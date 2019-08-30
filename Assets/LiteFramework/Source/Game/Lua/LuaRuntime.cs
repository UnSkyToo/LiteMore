using System;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.UI;
using LiteFramework.Interface.Lua;
using XLua;

namespace LiteFramework.Game.Lua
{
    public static class LuaRuntime
    {
        private static LuaEnv LuaEnv_ = null;
        private static ILuaMainEntity MainEntity_ = null;

        public static bool Startup()
        {
            LuaEnv_ = new LuaEnv();
            LuaEnv_.AddLoader(StandaloneLuaLoader);

            EventManager.Register<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.Register<EnterBackgroundEvent>(OnEnterBackgroundEvent);

            return true;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.UnRegister<EnterBackgroundEvent>(OnEnterBackgroundEvent);

            MainEntity_?.Shutdown();
            MainEntity_ = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            LuaEnv_.Dispose(true);
        }

        public static void Tick(float DeltaTime)
        {
            MainEntity_?.Tick(DeltaTime);
        }

        public static bool ExecuteMainLuaFile(string FileName)
        {
            LuaEnv_.DoString($"_lite_main_entity_ = require '{FileName}'", FileName);

            MainEntity_ = LuaEnv_.Global.GetInPath<ILuaMainEntity>("_lite_main_entity_");
            if (MainEntity_ == null)
            {
                LLogger.LWarning($"can't load {FileName}.lua file");
                return false;
            }

            var State = MainEntity_.Startup();
            if (!State)
            {
                LLogger.LWarning("lua main entity start failed");
                return false;
            }

            return true;
        }

        private static byte[] StandaloneLuaLoader(ref string LuaPath)
        {
#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
            var FullPath = $"scripts/{LuaPath.Replace('.', '/')}.lua";
#else
            var FullPath = $"scripts/{LuaPath.Replace('.', '/')}.lua.bytes";
#endif
            //LuaPath = Helper.PathHelper.GetAssetFullPath(FullPath);
            return AssetManager.CreateDataSync(FullPath);
        }

        public static void OpenLuaUI(LuaTable Desc, LuaTable LuaEntity)
        {
            var UIDesc = new UIDescriptor(
                Desc.GetInPath<string>("PrefabName"),
                Desc.GetInPath<bool>("OpenMore"),
                Desc.GetInPath<bool>("Cached"));

            var UI = new LuaBaseUI(LuaEntity);
            UIManager.OpenUI<LuaBaseUI>(UI, UIDesc, null);
        }

        public static void CloseLuaUI(LuaTable LuaEntity)
        {
            var UI = LuaEntity?.GetInPath<LuaBaseUI>("_CSEntity_");
            if (UI != null)
            {
                UIManager.CloseUI(UI);
            }
        }

        private static void OnEnterForegroundEvent(EnterForegroundEvent Msg)
        {
            MainEntity_?.EnterForeground();
        }

        private static void OnEnterBackgroundEvent(EnterBackgroundEvent Msg)
        {
            MainEntity_?.EnterBackground();
        }
    }
}