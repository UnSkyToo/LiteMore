using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct LaserBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public float BeginAngle { get; }
        public float EndAngle { get; }
        public float StepAngle { get; }
        public float Radius { get; }

        public LaserBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, float BeginAngle, float EndAngle, float StepAngle, float Radius)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.BeginAngle = BeginAngle;
            this.EndAngle = EndAngle;
            this.StepAngle = StepAngle;
            this.Radius = Radius;
        }
    }

    public class LaserBullet : BaseBullet
    {
        private FanShape Shape_;

        private readonly LineCaller LineCaller_;
        private float BeginAngle_;
        private float EndAngle_;
        private float StepAngle_;
        private float Radius_;

        public LaserBullet(Transform Trans, LaserBulletDescriptor Desc)
            : base(Trans, BulletType.Laser, Desc.BaseBulletDesc)
        {
            LineCaller_ = new LineCaller(Trans.GetComponent<LineRenderer>());
            BeginAngle_ = Desc.BeginAngle;
            EndAngle_ = Desc.EndAngle;
            StepAngle_ = Desc.StepAngle;
            Radius_ = Desc.Radius;

            Shape_ = new FanShape(Position, Radius_, BeginAngle_, BeginAngle_);
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (!IsAlive)
            {
                return;
            }

            Shape_.BeginAngle = Shape_.EndAngle;
            Shape_.EndAngle += StepAngle_ * DeltaTime;
            if (Shape_.EndAngle >= EndAngle_)
            {
                Shape_.EndAngle = EndAngle_;
                IsAlive = false;
            }

            var X = Mathf.Sin(Mathf.Deg2Rad * Shape_.EndAngle) * Radius_;
            var Y = Mathf.Cos(Mathf.Deg2Rad * Shape_.EndAngle) * Radius_;

            var TargetPos = new Vector2(X, Y);
            LineCaller_.DrawLine(new LineCallerPoint(Vector2.zero, Color.green, 3), new LineCallerPoint(TargetPos, Color.green, 10));

            CheckHit();
        }

        private void CheckHit()
        {
            Shape_.Center = Position;

            foreach (var Entity in NpcManager.GetNpcList())
            {
                if (!Entity.CanLocked())
                {
                    continue;
                }

                if (Shape_.Contains(Entity.Position))
                {
                    Entity.OnBulletHit(this);
                    LabelManager.AddNumberLabel(Entity.Position, NumberLabelType.Laser, Damage);
                }
            }
        }
    }
}