using System;

namespace LiteMore.Core
{
    public abstract class BaseEntity : BaseObject, IDisposable
    {
        public bool IsAlive { get; set; }

        protected BaseEntity()
            : base()
        {
            IsAlive = true;
        }

        public abstract void Dispose();
        public abstract void Tick(float Time);
    }
}