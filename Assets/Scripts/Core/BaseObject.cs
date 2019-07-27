namespace LiteMore.Core
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