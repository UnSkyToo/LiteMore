using XLua;

namespace LiteFramework.Game.Lua.Interface
{
    public interface ILuaUIEntity
    {
        void OnOpen();
        void OnClose();
        void OnShow();
        void OnHide();
        void OnTick(float DeltaTime);
    }
}