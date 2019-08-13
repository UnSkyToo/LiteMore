using LiteFramework.Helper;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct BackBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public Vector2 Direction { get; }
        public float Distance { get; }
        public Vector2 Size { get; }
        public float Speed { get; }

        public BackBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, Vector2 Direction, float Distance, Vector2 Size, float Speed)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.Direction = Direction;
            this.Distance = Distance;
            this.Size = Size;
            this.Speed = Speed;
        }
    }

    public class BackBullet : BaseBullet
    {
        private RectShape Shape_;

        private float Speed_;
        private Vector2 Direction_;
        private Vector2 Size_;
        private float Distance_;

        public BackBullet(Transform Trans, BackBulletDescriptor Desc)
            : base(Trans, BulletType.Back, Desc.BaseBulletDesc)
        {
            Distance_ = Desc.Distance;
            Direction_ = Desc.Direction;
            Speed_ = Desc.Speed;
            Size_ = Desc.Size;

            var Angle = MathHelper.GetUnityAngle(Direction_);
            Rotation = Quaternion.AngleAxis(Angle, Vector3.forward);

            Shape_ = new RectShape(Position, Size_, Rotation);
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (!IsAlive)
            {
                return;
            }

            Position += (Direction_ * Speed_ * DeltaTime);
            Distance_ -= Speed_ * DeltaTime;

            if (Distance_ <= 0)
            {
                IsAlive = false;
            }

            CheckHit();
        }

        private void CheckHit()
        {
            Shape_.Center = Position;
            //Shape_.Rotation = Rotation;

            foreach (var Entity in NpcManager.GetNpcList(Team.Opposite()))
            {
                if (!Entity.CanLocked())
                {
                    continue;
                }

                if (Shape_.Contains(Entity.Position))
                {
                    Entity.OnBulletHit(this);
                    LabelManager.AddNumberLabel(Entity.Position, NumberLabelType.Float, Damage);
                    Entity.Back((Entity.Position - Position).normalized * 30, 400);
                }
            }
        }
    }
}