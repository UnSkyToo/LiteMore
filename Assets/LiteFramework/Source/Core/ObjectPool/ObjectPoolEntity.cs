using System;
using System.Collections.Generic;

namespace LiteFramework.Core.ObjectPool
{
    public class ObjectPoolEntity<T> : BaseObjectPool where T : UnityEngine.Object
    {
        public int InfrequentCount { get; set; } = 2;

        protected Func<T> CreateFunc_ = null;
        protected Action<T> SpawnFunc_ = null;
        protected Action<T> RecycleFunc_ = null;
        protected Action<T> DisposeFunc_ = null;

        public ObjectPoolEntity(string PoolName, Func<T> CreateFunc, Action<T> SpawnFunc, Action<T> RecycleFunc, Action<T> DisposeFunc)
            : base(PoolName, typeof(T))
        {
            this.CreateFunc_ = CreateFunc;
            this.SpawnFunc_ = SpawnFunc;
            this.RecycleFunc_ = RecycleFunc;
            this.DisposeFunc_ = DisposeFunc;
        }

        public T Spawn()
        {
            foreach (var Cache in ObjectCacheList_)
            {
                if (!Cache.Value.Used)
                {
                    UsedCount++;
                    IdleCount--;
                    var TCache = Cache.Value as ObjectPoolCache<T>;
                    TriggerSpawn(TCache);
                    return TCache.Entity;
                }
            }

            var Obj = CreateFunc_.Invoke();
            var NewCache = new ObjectPoolCache<T>(Obj);
            ObjectCacheList_.Add(Obj.GetInstanceID(), NewCache);
            TriggerSpawn(NewCache);
            UsedCount++;

            return Obj;
        }

        public void Recycle(T Obj)
        {
            UsedCount--;
            IdleCount++;

            if (Obj != null && ObjectCacheList_.ContainsKey(Obj.GetInstanceID()))
            {
                TriggerRecycle(ObjectCacheList_[Obj.GetInstanceID()] as ObjectPoolCache<T>);
            }
        }

        public override void Dispose()
        {
            foreach (var Cache in ObjectCacheList_)
            {
                TriggerDispose(Cache.Value as ObjectPoolCache<T>);
            }

            ObjectCacheList_.Clear();
            UsedCount = 0;
            IdleCount = 0;

            CreateFunc_ = null;
            SpawnFunc_ = null;
            RecycleFunc_ = null;
            DisposeFunc_ = null;
        }

        public void DestroyUnusedObjects()
        {
            var DisposeKeys = new List<int>();

            foreach (var Cache in ObjectCacheList_)
            {
                if (!Cache.Value.Used)
                {
                    DisposeKeys.Add(Cache.Key);
                }
            }

            DisposeObjectList(DisposeKeys);
        }

        public void DestroyInfrequentObjects()
        {
            var DisposeKeys = new List<int>();

            foreach (var Cache in ObjectCacheList_)
            {
                if (!Cache.Value.Used && Cache.Value.Count <= InfrequentCount)
                {
                    DisposeKeys.Add(Cache.Key);
                }
            }

            DisposeObjectList(DisposeKeys);
        }

        private void DisposeObjectList(List<int> Keys)
        {
            foreach (var Key in Keys)
            {
                if (ObjectCacheList_.ContainsKey(Key))
                {
                    TriggerDispose(ObjectCacheList_[Key] as ObjectPoolCache<T>);
                    ObjectCacheList_.Remove(Key);
                }
            }
        }

        private void TriggerSpawn(ObjectPoolCache<T> Cache)
        {
            Cache.OnSpawn();
            SpawnFunc_?.Invoke(Cache.Entity);
        }

        private void TriggerRecycle(ObjectPoolCache<T> Cache)
        {
            Cache.OnRecycle();
            RecycleFunc_?.Invoke(Cache.Entity);
        }

        private void TriggerDispose(ObjectPoolCache<T> Cache)
        {
            DisposeFunc_?.Invoke(Cache.Entity);
        }
    }
}