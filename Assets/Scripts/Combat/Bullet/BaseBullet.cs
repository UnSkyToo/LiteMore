using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public enum BulletType : byte
    {
        Track,
        Laser,
        Bomb,
        Back,
        Trigger,
    }

    public struct BaseBulletDescriptor
    {
        public Vector2 Position { get; }
        public int Damage { get; }

        public BaseBulletDescriptor(Vector2 Position, int Damage)
        {
            this.Position = Position;
            this.Damage = Damage;
        }
    }

    public abstract class BaseBullet : GameEntity
    {
        public BulletType Type { get; }
        public int Damage { get; }

        protected BaseBullet(Transform Trans, BulletType Type, BaseBulletDescriptor Desc)
            : base(Trans)
        {
            this.Type = Type;
            this.Position = Desc.Position;
            this.Damage = Desc.Damage;
        }
    }
}