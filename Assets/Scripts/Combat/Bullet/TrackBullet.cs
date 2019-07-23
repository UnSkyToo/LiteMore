using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct TrackBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public string ResName { get; }
        public BaseNpc Target { get; }
        public float Speed { get; }

        public TrackBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, string ResName, BaseNpc Target, float Speed)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.ResName = ResName;
            this.Target = Target;
            this.Speed = Speed;
        }
    }

    public class TrackBullet : BaseBullet
    {
        public bool IsMove { get; private set; }
        public float Speed { get; private set; }

        private BaseNpc Target_;
        private Vector2 BeginPos_;
        private Vector2 EndPos_;
        private float MoveTime_;
        private float MoveTotalTime_;

        public TrackBullet(Transform Trans, TrackBulletDescriptor Desc)
            : base(Trans, BulletType.Track, Desc.BaseBulletDesc)
        {
            IsMove = false;
            Speed = Desc.Speed;
            Trans.GetComponent<Animator>().SetTrigger(Desc.ResName);

            Attack(Desc.Target);
        }

        public override void Tick(float DeltaTime)
        {
            if (Target_ == null)
            {
                return;
            }

            if (!Target_.IsAlive)
            {
                IsAlive = false;
            }
            else
            {
                MoveTo(Target_.Position);
                if (MoveTotalTime_ < 0.016f)
                {
                    Position = Target_.Position;
                    Target_.OnBulletHit(this);
                    LabelManager.AddNumberLabel(Target_.Position, NumberLabelType.Float, Damage);
                    IsAlive = false;
                }
            }

            if (!IsAlive)
            {
                return;
            }

            MoveTime_ += DeltaTime;
            var T = MoveTime_ / MoveTotalTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                IsMove = false;
            }

            Position = Vector2.Lerp(BeginPos_, EndPos_, T);
            base.Tick(DeltaTime);
        }

        private void Attack(BaseNpc Target)
        {
            Target_ = Target;
            Target_.OnLocking(this);
            MoveTo(Target_.Position);
        }

        private void MoveTo(Vector2 TargetPos)
        {
            IsMove = true;
            MoveTime_ = 0;
            BeginPos_ = Position;
            EndPos_ = TargetPos;
            MoveTotalTime_ = Vector2.Distance(BeginPos_, EndPos_) / Speed;

            Rotation = Quaternion.AngleAxis(90 - MathHelper.GetAngle(Position, TargetPos), Vector3.forward);
        }
    }
}
