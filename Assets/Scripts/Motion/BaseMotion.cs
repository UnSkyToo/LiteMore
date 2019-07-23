using UnityEngine;

namespace LiteMore.Motion
{
    public abstract class BaseMotion
    {
        public uint ID { get; }
        public Transform Master { get; set; }
        public bool IsEnd { get; protected set; }

        protected BaseMotion()
        {
            ID = IDGenerator.Get();
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