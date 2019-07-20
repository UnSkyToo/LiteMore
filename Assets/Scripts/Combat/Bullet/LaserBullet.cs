using LiteMore.Combat.Npc;
using LiteMore.Extend;
using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public class LaserBullet : BulletBase
    {
        private readonly LineCaller LineCaller_;
        private float StartAngle_;
        private float EndAngle_;
        private float PreAngle_;
        private float CurAngle_;
        private float StepAngle_;
        private float Radius_;

        public LaserBullet(Transform Trans)
            : base(BulletType.Laser, Trans)
        {
            LineCaller_ = new LineCaller(Trans.GetComponent<LineRenderer>());
            StartAngle_ = 180;
            EndAngle_ = 360;
            CurAngle_ = StartAngle_;
            PreAngle_ = StartAngle_;
            StepAngle_ = 300;
            Radius_ = 500;

            Damage = 100;
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            PreAngle_ = CurAngle_;
            CurAngle_ += StepAngle_ * DeltaTime;
            if (CurAngle_ >= EndAngle_)
            {
                CurAngle_ = EndAngle_;
                IsAlive = false;
            }

            var X = Mathf.Sin(Mathf.Deg2Rad * CurAngle_) * Radius_;
            var Y = Mathf.Cos(Mathf.Deg2Rad * CurAngle_) * Radius_;

            var TargetPos = new Vector2(X, Y);
            LineCaller_.DrawLine(new LineCallerPoint(Vector2.zero, Color.green, 3), new LineCallerPoint(TargetPos, Color.green, 10));

            CheckHit();
        }

        private void CheckHit()
        {
            foreach (var Entity in NpcManager.GetNpcList())
            {
                if (Entity.Hp <= 0 || Entity.ForecastHp <= 0)
                {
                    continue;
                }

                var TargetPos = Entity.Position;
                if (Vector2.Distance(TargetPos, Position) > Radius_)
                {
                    continue;
                }

                var Angle = MathHelper.GetAngle(Position, TargetPos);
                if (Angle >= PreAngle_ && Angle <= CurAngle_)
                {
                    Entity.OnBulletHit(this);
                }
            }
        }
    }
}