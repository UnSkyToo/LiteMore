using LiteMore.Core;
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
        public string Name { get; }
        public Vector2 Position { get; }
        public float Damage { get; }
        public CombatTeam Team { get; }

        public BaseBulletDescriptor(string Name, Vector2 Position, CombatTeam Team, float Damage)
        {
            this.Name = Name;
            this.Position = Position;
            this.Damage = Damage;
            this.Team = Team;
        }
    }

    public abstract class BaseBullet : GameEntity
    {
        public BulletType Type { get; }
        public CombatTeam Team { get; }
        public float Damage { get; }

        protected BaseBullet(Transform Trans, BulletType Type, BaseBulletDescriptor Desc)
            : base(Desc.Name, Trans)
        {
            this.Type = Type;
            this.Position = Desc.Position;
            this.Damage = Desc.Damage;
            this.Team = Desc.Team;
        }
    }
}