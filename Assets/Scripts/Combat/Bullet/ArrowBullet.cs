using LiteFramework.Core.BezierCurve;
using LiteFramework.Game.Audio;
using LiteFramework.Helper;
using LiteMore.Combat.AI.Filter;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct ArrowBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public Vector2 TargetPos { get; }
        public float Speed { get; }
        public Color ArrowColor { get; }

        public ArrowBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, Vector2 TargetPos, float Speed, Color ArrowColor)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.TargetPos = TargetPos;
            this.Speed = Speed;
            this.ArrowColor = ArrowColor;
        }
    }

    public class ArrowBullet : BaseBullet
    {
        private readonly Vector2 StartPos_;
        private readonly Vector2 EndPos_;
        private readonly IBezierCurve BezierCurve_;
        private readonly float BezierTime_;
        private float CurrentTime_;
        private float StayTime_;
        private bool IsStaying_;

        public ArrowBullet(Transform Trans, ArrowBulletDescriptor Desc)
            : base(Trans, BulletType.Arrow, Desc.BaseBulletDesc)
        {
            StartPos_ = Desc.BaseBulletDesc.Position;
            EndPos_ = Desc.TargetPos;

            var MaxY = Mathf.Max(StartPos_.y, EndPos_.y);
            var DisX = Mathf.Abs(StartPos_.x - EndPos_.x);
            var ControlPos = new Vector2((StartPos_.x + EndPos_.x) / 2, MaxY + DisX / 2);

            BezierCurve_ = BezierCurveFactory.CreateBezierCurve(StartPos_, ControlPos, EndPos_);
            BezierTime_ = DisX / Mathf.Max(Desc.Speed, 1);
            CurrentTime_ = 0;

            StayTime_ = 1.0f;
            IsStaying_ = false;

            GetComponent<SpriteRenderer>().color = Desc.ArrowColor;

            var NextPos = BezierCurve_.Lerp(0.01f);
            SetRotationWithNextPos(NextPos);

            //AudioManager.PlaySound("audio/tf_arrow_bul.mp3");
        }

        public override void Tick(float DeltaTime)
        {
            if (IsStaying_)
            {
                StayTime_ -= DeltaTime;
                if (StayTime_ <= 0)
                {
                    IsAlive = false;
                }
                return;
            }

            CurrentTime_ += DeltaTime;
            
            var NextPos = BezierCurve_.Lerp(CurrentTime_ / BezierTime_);
            SetRotationWithNextPos(NextPos);
            Position = NextPos;

            if (CurrentTime_ >= BezierTime_)
            {
                OnAttack();
            }
        }

        private void OnAttack()
        {
            var Targets = FilterHelper.Find(new FilterRule(FilterTeamType.Enemy, FilterRangeType.InDistance, FilterNpcType.Nearest), new FilterArgs()
            {
                Team = Team,
                Position = Position,
                Radius = 5,
            });

            if (Targets.Count > 0)
            {
                //AudioManager.PlaySound("audio/tf_arrow_hit.mp3");
                Targets[0].Action.OnBulletHit(this);
            }

            IsStaying_ = true;
            StayTime_ = 1.0f;
        }

        private void SetRotationWithNextPos(Vector2 NextPos)
        {
            var Angle = MathHelper.GetUnityAngle(Position, NextPos);
            Rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
        }
    }
}