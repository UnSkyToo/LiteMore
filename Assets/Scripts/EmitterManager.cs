using System.Collections.Generic;
using LiteMore.Extend;
using UnityEngine;

namespace LiteMore
{
    public class EmitterRandData<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        private readonly System.Func<T, T, T> CreateFunc_;

        public EmitterRandData(System.Func<T, T, T> CreateFunc)
        {
            Min = default;
            Max = default;
            CreateFunc_ = CreateFunc;
        }

        public T Get()
        {
            return CreateFunc_(Min, Max);
        }
    }

    public class EmitterRandFloat : EmitterRandData<float>
    {
        public EmitterRandFloat(float Min, float Max)
            : base(Random.Range)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public class EmitterRandInt : EmitterRandData<int>
    {
        public EmitterRandInt(int Min, int Max)
            : base(Random.Range)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public class EmitterRandVector2 : EmitterRandData<Vector2>
    {
        public EmitterRandVector2(Vector2 Min, Vector2 Max)
            : base((A, B) => new Vector2(Random.Range(A.x, B.x), Random.Range(A.y, B.y)))
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public abstract class EmitterBase : EntityBase
    {
        public bool IsPause { get; set; }
        public float Interval { get; set; }
        public uint EmittedCount { get; set; }
        public int TriggerCount { get; set; }

        private float Time_;

        protected EmitterBase()
            : base()
        {
            IsPause = false;
            Interval = 1;
            Time_ = 0;
            EmittedCount = 1;
            TriggerCount = -1;
        }

        public override void Create()
        {
        }

        public override void Destroy()
        {
        }

        public override void Tick(float DeltaTime)
        {
            if (IsPause || !IsAlive)
            {
                return;
            }

            Time_ += DeltaTime;
            if (Time_ >= Interval)
            {
                Time_ -= Interval;

                for (var Index = 0u; Index < EmittedCount; ++Index)
                {
                    OnEmitted(Index, EmittedCount);
                }

                if (TriggerCount > 0)
                {
                    TriggerCount--;
                    if (TriggerCount == 0)
                    {
                        IsPause = true;
                        IsAlive = false;
                    }
                }
            }
        }

        protected abstract void OnEmitted(uint Cur, uint Max);
    }

    public class NpcNormalEmitter : EmitterBase
    {
        public EmitterRandFloat RadiusAttr { get; set; }
        public EmitterRandFloat SpeedAttr { get; set; }
        public EmitterRandInt HpAttr { get; set; }

        private Transform ObjInner_;
        private LineCaller CallerInner_;

        private Transform ObjOuter_;
        private LineCaller CallerOuter_;

        public NpcNormalEmitter()
            : base()
        {
        }

        public override void Create()
        {
            ObjInner_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line")).transform;
            ObjInner_.name = $"NpcNormalInner<{ID}>";
            ObjInner_.SetParent(GameObject.Find("Emitter").transform, false);
            ObjInner_.localPosition = Vector3.zero;

            CallerInner_ = new LineCaller(ObjInner_.GetComponent<LineRenderer>());
            CallerInner_.DrawCircle(new LineCallerPoint(Position, Color.red), RadiusAttr.Min);

            ObjOuter_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line")).transform;
            ObjOuter_.name = $"NpcNormalOuter<{ID}>";
            ObjOuter_.SetParent(GameObject.Find("Emitter").transform, false);
            ObjOuter_.localPosition = Vector3.zero;

            CallerOuter_ = new LineCaller(ObjOuter_.GetComponent<LineRenderer>());
            CallerOuter_.DrawCircle(new LineCallerPoint(Position, Color.blue), RadiusAttr.Max);
        }

        public override void Destroy()
        {
            Object.Destroy(ObjInner_.gameObject);
            Object.Destroy(ObjOuter_.gameObject);
        }

        protected override void OnEmitted(uint Cur, uint Max)
        {
            var Radius = RadiusAttr.Get();
            var Angle = Random.Range(0, Mathf.PI * 2);
            var Pos = Position + new Vector2(Mathf.Sin(Angle) * Radius, Mathf.Cos(Angle) * Radius);

            var Entity = NpcManager.AddNpc(Pos);
            Entity.Speed = SpeedAttr.Get();
            Entity.SetHp(HpAttr.Get());
            Entity.MoveTo(new Vector2(Screen.width / 2.0f - 100, Random.Range(-100, 100)));
        }
    }

    public class BulletNormalEmitter : EmitterBase
    {
        public EmitterRandFloat RadiusAttr { get; set; }
        public EmitterRandFloat SpeedAttr { get; set; }
        public EmitterRandInt DamageAttr { get; set; }
        public string ResName { get; set; }

        public BulletNormalEmitter()
            : base()
        {
        }

        protected override void OnEmitted(uint Cur, uint Max)
        {
            var Target = NpcManager.GetRandomNpc();
            if (Target != null)
            {
                var Radius = RadiusAttr.Get();
                var Angle = Random.Range(0, Mathf.PI * 2);
                var Pos = Position + new Vector2(Mathf.Sin(Angle) * Radius, Mathf.Cos(Angle) * Radius);

                var Entity = BulletManager.AddBullet(ResName, Pos);
                Entity.Speed = SpeedAttr.Get();
                Entity.Damage = DamageAttr.Get();
                Entity.Attack(Target);
            }
        }
    }

    public static class EmitterManager
    {
        private static readonly List<EmitterBase> EmitterList_ = new List<EmitterBase>();

        public static bool Startup()
        {
            EmitterList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in EmitterList_)
            {
                Entity.Destroy();
            }

            EmitterList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = EmitterList_.Count - 1; Index >= 0; --Index)
            {
                EmitterList_[Index].Tick(DeltaTime);

                if (!EmitterList_[Index].IsAlive)
                {
                    EmitterList_[Index].Destroy();
                    EmitterList_.RemoveAt(Index);
                }
            }
        }

        public static EmitterBase AddEmitter(EmitterBase Emitter)
        {
            EmitterList_.Add(Emitter);
            Emitter.Create();
            return Emitter;
        }

        public static int GetCount()
        {
            return EmitterList_.Count;
        }
    }
}