namespace LiteFramework.Extend.Debug
{
    internal abstract class BaseDebugWindow
    {
        protected BaseDebugWindow()
        {
        }

        internal abstract bool Startup();

        internal abstract void Shutdown();

        internal abstract void Tick(float DeltaTime);

        internal abstract void Draw();
    }
}