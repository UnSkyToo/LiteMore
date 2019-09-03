using System;
using System.Collections.Generic;
using System.Reflection;
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

            EventManager.OnSend += OnLuaEvent;
            EventManager.Register<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.Register<EnterBackgroundEvent>(OnEnterBackgroundEvent);

            return true;
        }

        public static void Shutdown()
        {
            EventList_.Clear();
            EventManager.UnRegister<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.UnRegister<EnterBackgroundEvent>(OnEnterBackgroundEvent);
            EventManager.OnSend -= OnLuaEvent;

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

        private static readonly Dictionary<string, Dictionary<LuaTable, Action<LuaTable, LuaTable>>> EventList_ = new Dictionary<string, Dictionary<LuaTable, Action<LuaTable, LuaTable>>>();
        public static void RegisterEvent(string EventName, LuaTable LuaEntity, Action<LuaTable, LuaTable> Callback)
        {
            if (!EventList_.ContainsKey(EventName))
            {
                EventList_.Add(EventName, new Dictionary<LuaTable, Action<LuaTable, LuaTable>>());
            }

            if (!EventList_[EventName].ContainsKey(LuaEntity))
            {
                EventList_[EventName].Add(LuaEntity, Callback);
            }
        }

        public static void UnRegisterEvent(string EventName, LuaTable LuaEntity)
        {
            if (EventList_.ContainsKey(EventName))
            {
                EventList_[EventName][LuaEntity] = null;
                EventList_[EventName].Remove(LuaEntity);
                if (EventList_[EventName].Count == 0)
                {
                    EventList_.Remove(EventName);
                }
            }
        }

        public static void UnRegisterAllEvent(LuaTable LuaEntity)
        {
            foreach (var Events in EventList_)
            {
                foreach (var Evt in Events.Value)
                {
                    if (Equals(Evt.Key, LuaEntity))
                    {
                        Events.Value[LuaEntity] = null;
                        Events.Value.Remove(LuaEntity);
                        break;
                    }
                }
            }
        }

        private static void OnLuaEvent(BaseEvent Event)
        {
            if (!EventList_.ContainsKey(Event.EventName))
            {
                return;
            }

            var Properties = Event.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var Table = LuaEnv_.NewTable();

            foreach (var Property in Properties)
            {
                Table.SetInPath(Property.Name, Property.GetValue(Event));
            }

            foreach (var Evt in EventList_[Event.EventName])
            {
                Evt.Value.Invoke(Evt.Key, Table);
            }
        }
    }
}