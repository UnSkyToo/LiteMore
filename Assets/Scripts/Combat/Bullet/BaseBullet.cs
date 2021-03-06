﻿using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct BaseBulletDescriptor
    {
        public string Name { get; }
        public Vector2 Position { get; }
        public CombatTeam Team { get; }
        public float Damage { get; }

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
            this.Team = Desc.Team;
            this.Damage = Desc.Damage;
        }

        public override string ToString()
        {
            return $"{Name}-{Type}-{Team}-{ID}";
        }

        public override void Dispose()
        {
            BulletManager.DisposeBullet(this);
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}