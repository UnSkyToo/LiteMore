namespace LiteFramework.Interface
{
    public interface ILogic : ITick
    {
        bool Startup();
        void Shutdown();
    }
}