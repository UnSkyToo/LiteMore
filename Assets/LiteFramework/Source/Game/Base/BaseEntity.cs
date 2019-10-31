using System;
using LiteFramework.Core.Base;
using LiteFramework.Interface;

namespace LiteFramework.Game.Base
{
    public abstract class BaseEntity : BaseObject, IDisposable, ITick
    {
        public string Name { get; protected set; }
        public bool IsAlive { get; set; }

        protected BaseEntity(string Name)
            : base()
        {
            this.Name = Name;
            this.IsAlive = true;
        }

        public abstract void Dispose();
        public abstract void Tick(float DeltaTime);
    }
}