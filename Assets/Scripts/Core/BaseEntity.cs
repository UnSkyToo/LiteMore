using System;
using LiteFramework.Core.Base;

namespace LiteMore.Core
{
    public abstract class BaseEntity : BaseObject, IDisposable
    {
        public string Name { get; }
        public bool IsAlive { get; set; }

        protected BaseEntity(string Name)
            : base()
        {
            this.Name = Name;
            this.IsAlive = true;
        }

        public abstract void Dispose();
        public abstract void Tick(float Time);
    }
}