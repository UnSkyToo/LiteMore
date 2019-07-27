using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Motion
{
    public abstract class BaseMotion : BaseObject
    {
        public Transform Master { get; set; }
        public bool IsEnd { get; protected set; }

        protected BaseMotion()
            : base()
        {
            IsEnd = true;
        }

        public void Stop()
        {
            IsEnd = true;
        }

        public abstract void Enter();

        public abstract void Tick(float DeltaTime);

        public abstract void Exit();
    }
}