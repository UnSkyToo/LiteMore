namespace LiteFramework.Game.Lua.Interface
{
    public interface ILuaMainEntity
    {
        bool Startup();
        void Shutdown();
        void Tick(float DeltaTime);
        void EnterForeground();
        void EnterBackground();
    }
}