namespace LiteFramework.Extend.Debug
{
    internal abstract class BaseDebuggerDrawItem : IDebuggerDrawItem
    {
        public string Name { get; set; }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract void Draw();
    }
}