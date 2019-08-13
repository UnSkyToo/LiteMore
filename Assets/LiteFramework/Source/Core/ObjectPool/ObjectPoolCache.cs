using UnityEngine;

namespace LiteFramework.Core.ObjectPool
{
    public class ObjectPoolCache
    {
        public GameObject Obj { get; set; }
        public bool Used { get; set; }
        public uint Count { get; set; }

        public ObjectPoolCache(GameObject Obj)
        {
            this.Obj = Obj;
            this.Used = false;
            this.Count = 1;
        }
    }
}