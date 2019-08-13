namespace LiteFramework.Core.Base
{
    public abstract class BaseObject
    {
        public uint ID { get; }

        protected BaseObject()
        {
            ID = IDGenerator.Get();
        }
    }
}