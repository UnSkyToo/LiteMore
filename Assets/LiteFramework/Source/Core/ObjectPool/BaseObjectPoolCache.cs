using System;
using UnityEngine;

namespace LiteFramework.Core.ObjectPool
{
    public abstract class BaseObjectPoolCache
    {
        public bool Used { get; private set; }
        public uint Count { get; private set; }
        public float TotalTime { get; private set; }
        public DateTime LastSpawnTime { get; private set; }
        public DateTime LastRecycleTime { get; private set; }
        private float Time_;

        protected BaseObjectPoolCache()
        {
            this.Used = false;
            this.Count = 0;
            this.TotalTime = 0;
            this.LastSpawnTime = DateTime.MinValue;
            this.LastRecycleTime = DateTime.MinValue;
            this.Time_ = 0;
        }

        public void OnSpawn()
        {
            if (Used)
            {
                return;
            }

            Used = true;
            Count++;
            Time_ = Time.time;
            LastSpawnTime = DateTime.Now;
        }

        public void OnRecycle()
        {
            if (!Used)
            {
                return;
            }

            Used = false;
            TotalTime += (Time.time - Time_);
            LastRecycleTime = DateTime.Now;
        }
    }
}