using UnityEngine;

namespace LiteMore.Combat
{
    public abstract class EntityBase
    {
        private static uint IDNext = 1;

        public uint ID { get; }
        public bool IsAlive { get; set; }
        public Vector2 Position { get; set; }

        protected EntityBase()
        {
            ID = IDNext++;
            IsAlive = true;
            Position = Vector2.zero;
        }

        public abstract void Create();

        public abstract void Destroy();

        public abstract void Tick(float DeltaTime);
    }

    public abstract class GameEntity : EntityBase
    {
        private Vector2 Scale_;
        public Vector2 Scale
        {
            get => Scale_;
            set
            {
                Scale_ = value;
                Transform_.localScale = value;
            }
        }

        private Quaternion Rotation_;
        public Quaternion Rotation
        {
            get => Rotation_;
            set
            {
                Rotation_ = value;
                Transform_.localRotation = value;
            }
        }

        private Transform Transform_;

        protected GameEntity(Transform Trans)
            : base()
        {
            Transform_ = Trans;
            Transform_.name = $"<{ID}>";

            Scale = Vector2.one;
            Rotation = Quaternion.identity;
        }

        public override void Tick(float DeltaTime)
        {
            Transform_.localPosition = Position;
        }

        public override void Create()
        {
        }

        public override void Destroy()
        {
            IsAlive = false;

            if (Transform_ != null)
            {
                Object.Destroy(Transform_.gameObject);
                Transform_ = null;
            }
        }

        public T GetComponent<T>()
        {
            return Transform_.GetComponent<T>();
        }

        public T GetComponent<T>(string Path)
        {
            return Transform_.Find(Path).GetComponent<T>();
        }
    }
}