using LiteMore.Combat.Npc;
using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public class TrackBullet : BulletBase
    {
        public float Speed { get; set; }
        public bool IsMove { get; private set; }

        protected Vector2 BeginPos_;
        protected Vector2 EndPos_;
        protected float MoveTime_;
        protected float MoveTotalTime_;

        private NpcBase Target_;

        public TrackBullet(Transform Trans, string ResName)
            : base(BulletType.Track, Trans)
        {
            IsMove = false;
            Speed = 100;
            Trans.GetComponent<Animator>().SetTrigger(ResName);
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

        public void Attack(NpcBase Target)
        {
            Target_ = Target;
            Target_.OnLocking(this);
            MoveTo(Target_.Position);
        }

        public void MoveTo(Vector2 TargetPos)
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
