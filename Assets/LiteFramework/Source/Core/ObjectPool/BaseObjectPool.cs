using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteFramework.Core.ObjectPool
{
    public abstract class BaseObjectPool : IDisposable
    {
        public string FullName { get; }
        public string PoolName { get; }
        public Type ObjectType { get; }
        public int Capacity => ObjectCacheList_.Count;
        public int UsedCount { get; protected set; }
        public int IdleCount { get; protected set; }

        protected readonly Dictionary<int, BaseObjectPoolCache> ObjectCacheList_ = null;

        protected BaseObjectPool(string PoolName, Type ObjectType)
        {
            this.FullName = $"{ObjectType.Name}_{PoolName}";
            this.PoolName = PoolName;
            this.ObjectType = ObjectType;
            this.ObjectCacheList_ = new Dictionary<int, BaseObjectPoolCache>();
            this.UsedCount = 0;
            this.IdleCount = 0;
        }

        public abstract void Dispose();

        public BaseObjectPoolCache[] GetObjectCacheList()
        {
            return ObjectCacheList_.Values.ToArray();
        }
    }
}