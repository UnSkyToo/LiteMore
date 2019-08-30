namespace LiteFramework.Interface.Lua
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