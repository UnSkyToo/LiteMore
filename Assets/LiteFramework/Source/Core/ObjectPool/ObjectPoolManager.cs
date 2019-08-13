using System.Collections.Generic;
using UnityEngine;

namespace LiteFramework.Core.ObjectPool
{
    public static class ObjectPoolManager
    {
        private static readonly Dictionary<string, ObjectPoolEntity> Pools_ = new Dictionary<string, ObjectPoolEntity>();

        public static bool Startup()
        {
            Pools_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Pool in Pools_)
            {
                Pool.Value.DestroyObjects();
            }
            Pools_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
        }

        public static ObjectPoolEntity AddPool(string PoolName, GameObject Prefab)
        {
            if (!Pools_.ContainsKey(PoolName))
            {
                var Pool = new ObjectPoolEntity(PoolName, Prefab);
                Pools_.Add(PoolName, Pool);
                return Pool;
            }

            return Pools_[PoolName];
        }

        public static void DeletePool(string PoolName)
        {
            if (Pools_.ContainsKey(PoolName))
            {
                Pools_[PoolName].DestroyObjects();
                Pools_.Remove(PoolName);
            }
        }

        public static void DeletePool(ObjectPoolEntity Pool)
        {
            if (Pool != null)
            {
                DeletePool(Pool.PoolName);
            }
        }
    }
}