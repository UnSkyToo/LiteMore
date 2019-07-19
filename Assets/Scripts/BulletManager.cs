using System.Collections.Generic;
using UnityEngine;

namespace LiteMore
{
    public abstract class BulletBase : GameEntity
    {
        public int Damage { get; set; }

        protected BulletBase(Transform Trans)
            : base(Trans)
        {
            Damage = 1;
        }
    }

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
            : base(Trans)
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
            LineCaller_.DrawLine(new LineCallerPoint(Vector2.zero, Color.green, 1), new LineCallerPoint(TargetPos, Color.green, 10));

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

    public class Bullet : BulletBase
    {
        public float Speed { get; set; }
        public bool IsMove { get; private set; }

        protected Vector2 BeginPos_;
        protected Vector2 EndPos_;
        protected float MoveTime_;
        protected float MoveTotalTime_;

        private Npc Target_;

        public Bullet(Transform Trans, string ResName)
            : base(Trans)
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

        public void Attack(Npc Target)
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

    public static class BulletManager
    {
        private static Transform BulletRoot_;
        private static GameObject ModelPrefab_;
        private static List<BulletBase> BulletList_;

        public static bool Startup()
        {
            BulletRoot_ = GameObject.Find("Bullet").transform;

            ModelPrefab_ = Resources.Load<GameObject>("Prefabs/Bullet");
            if (ModelPrefab_ == null)
            {
                Debug.Log("BulletManager : null model prefab");
                return false;
            }

            BulletList_ = new List<BulletBase>();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in BulletList_)
            {
                Entity.Destroy();
            }
            BulletList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = BulletList_.Count - 1; Index >= 0; --Index)
            {
                BulletList_[Index].Tick(DeltaTime);

                if (!BulletList_[Index].IsAlive)
                {
                    BulletList_[Index].Destroy();
                    BulletList_.RemoveAt(Index);
                }
            }
        }

        public static Bullet AddBullet(string ResName, Vector2 Position)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(BulletRoot_, false);
            Obj.transform.localPosition = Position;

            var Entity = new Bullet(Obj.transform, ResName);
            Entity.Create();
            BulletList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static LaserBullet AddLaserBullet(Vector2 Position)
        {
            var Obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line"));
            Obj.transform.SetParent(BulletRoot_, false);
            Obj.transform.localPosition = Position;

            var Entity = new LaserBullet(Obj.transform);
            Entity.Create();
            BulletList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static int GetCount()
        {
            return BulletList_.Count;
        }
    }
}