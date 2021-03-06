﻿using LiteFramework.Game.Asset;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Npc.Module;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct BombBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public Vector2 TargetPos { get; }
        public float Speed { get; }
        public float Radius { get; }

        public BombBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, Vector2 TargetPos, float Speed, float Radius)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.TargetPos = TargetPos;
            this.Speed = Speed;
            this.Radius = Radius;
        }
    }

    public class BombBullet : BaseBullet
    {
        private CircleShape Shape_;

        private float Speed_;
        private BaseSfx BombSfx_;
        private Vector2 OriginPos_;
        private Vector2 TargetPos_;
        private float Time_;
        private float MaxTime_;
        private bool IsBomb_;
        private float Radius_;

        private GameObject RadiusObj_;

        public BombBullet(Transform Trans, BombBulletDescriptor Desc)
            : base(Trans, BulletType.Bomb, Desc.BaseBulletDesc)
        {
            Radius_ = Desc.Radius;
            Speed_ = Desc.Speed;
            OriginPos_ = Position;
            TargetPos_ = Desc.TargetPos;
            Time_ = 0;
            MaxTime_ = Vector2.Distance(OriginPos_, TargetPos_) / Speed_;
            IsBomb_ = false;

            RadiusObj_ = AssetManager.CreatePrefabSync(new AssetUri("prefabs/bv0.prefab"));
            MapManager.AddToGroundLayer(RadiusObj_.transform);
            RadiusObj_.transform.localPosition = TargetPos_;
            var SR = RadiusObj_.GetComponent<SpriteRenderer>();
            SR.color = Color.red;
            SR.size = new Vector2(Radius_ * 2, Radius_ * 2);

            Shape_ = new CircleShape(Radius_);
        }

        public override void Dispose()
        {
            AssetManager.DeleteAsset(RadiusObj_);
            base.Dispose();
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsBomb_)
            {
                if (BombSfx_.IsEnd())
                {
                    IsAlive = false;
                }
                return;
            }

            Time_ += DeltaTime;
            var T = Time_ / MaxTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                Bomb();
            }

            Position = Vector2.Lerp(OriginPos_, TargetPos_, T);
        }

        private void Bomb()
        {
            IsBomb_ = true;
            BombSfx_ = SfxManager.PlaySkySfx(Position, "prefabs/sfx/bombsfx.prefab");

            foreach (var Entity in NpcManager.GetNpcList(Team.Opposite()))
            {
                if (!Entity.Action.CanLocked())
                {
                    continue;
                }

                if (Shape_.Contains(Position, Entity.Position, Quaternion.identity))
                {
                    Entity.Action.OnBulletHit(this);
                    //LabelManager.AddNumberLabel(Entity.Position, NumberLabelType.Bomb, Damage);
                    Entity.Action.Back((Entity.Position - Position).normalized * 80, 400);
                }
            }
        }
    }
}