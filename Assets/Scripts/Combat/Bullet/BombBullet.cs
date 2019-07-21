using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public class BombBullet : BulletBase
    {
        private float Speed_;
        private Sfx BombSfx_;
        private Vector2 OriginPos_;
        private Vector2 TargetPos_;
        private float Time_;
        private float MaxTime_;
        private bool IsBomb_;
        private float Radius_;

        private GameObject RadiusObj_;

        public BombBullet(Transform Trans, Vector2 TargetPos)
            : base(BulletType.Bomb, Trans)
        {
            Damage = 500;

            Speed_ = 200;
            OriginPos_ = Trans.localPosition;
            TargetPos_ = TargetPos;
            Time_ = 0;
            MaxTime_ = Vector2.Distance(OriginPos_, TargetPos) / Speed_;
            IsBomb_ = false;

            Radius_ = 250;

            RadiusObj_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/bv0"));
            RadiusObj_.transform.SetParent(GameObject.Find("Sfx").transform, false);
            RadiusObj_.transform.localPosition = TargetPos;
            var SR = RadiusObj_.GetComponent<SpriteRenderer>();
            SR.color = Color.red;
            SR.size = new Vector2(Radius_ * 2, Radius_ * 2);
        }

        public override void Destroy()
        {
            base.Destroy();

            Object.Destroy(RadiusObj_);
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
            BombSfx_ = SfxManager.AddSfx("Prefabs/Sfx/BombSfx", Position);

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

                Entity.OnBulletHit(this);
            }
        }
    }
}