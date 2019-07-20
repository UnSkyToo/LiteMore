using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public enum BulletType : byte
    {
        Track,
        Laser,
        Bomb,
    }

    public abstract class BulletBase : GameEntity
    {
        public BulletType Type { get; }
        public int Damage { get; set; }

        protected BulletBase(BulletType Type, Transform Trans)
            : base(Trans)
        {
            this.Type = Type;
            this.Damage = 1;
        }
    }
}