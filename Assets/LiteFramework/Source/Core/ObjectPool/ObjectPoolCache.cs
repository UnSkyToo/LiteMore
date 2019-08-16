namespace LiteFramework.Core.ObjectPool
{
    public class ObjectPoolCache<T> : BaseObjectPoolCache where T : UnityEngine.Object
    {
        public T Entity { get; }

        public ObjectPoolCache(T Entity)
            : base()
        {
            this.Entity = Entity;
        }
    }
}