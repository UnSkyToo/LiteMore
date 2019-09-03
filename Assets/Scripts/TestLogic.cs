using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Game.Lua;
using LiteFramework.Helper;
using LiteFramework.Interface;
using LiteMore.Combat;
using LiteMore.UI;

namespace LiteMore
{
    public class TestLogic : ILogic
    {
        public bool Startup()
        {
            LuaRuntime.ExecuteMainLuaFile("main");

            EventManager.Send(new NpcAttrChangedEvent(null, NpcAttrIndex.AddHp, -1, 1));
            return true;
        }

        public void Shutdown()
        {
        }

        public void Tick(float DeltaTime)
        {
        }
    }
}