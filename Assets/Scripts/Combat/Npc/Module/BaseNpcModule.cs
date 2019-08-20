using System;
using LiteMore.Interface;

namespace LiteMore.Combat.Npc.Module
{
    public abstract class BaseNpcModule : ITick, IDisposable
    {
        public BaseNpc Master { get; }

        protected BaseNpcModule(BaseNpc Master)
        {
            this.Master = Master;
        }

        public virtual void Tick(float DeltaTime)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}