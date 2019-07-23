using LiteMore.Combat.Bullet;
using LiteMore.Combat.Npc;
using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Emitter
{
    public class NpcNormalEmitter : BaseEmitter
    {
        public EmitterRandFloat RadiusAttr { get; set; }
        public EmitterRandFloat SpeedAttr { get; set; }
        public EmitterRandInt HpAttr { get; set; }
        public EmitterRandInt DamageAttr { get; set; }

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
            ObjInner_.SetParent(Configure.EmitterRoot, false);
            ObjInner_.localPosition = Vector3.zero;

            CallerInner_ = new LineCaller(ObjInner_.GetComponent<LineRenderer>());
            CallerInner_.DrawCircle(new LineCallerPoint(Position, Color.red), RadiusAttr.Min);

            ObjOuter_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line")).transform;
            ObjOuter_.name = $"NpcNormalOuter<{ID}>";
            ObjOuter_.SetParent(Configure.EmitterRoot, false);
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

            var InitAttr = NpcManager.GenerateInitAttr(SpeedAttr.Get(), HpAttr.Get(), DamageAttr.Get(), 1);
            var Entity = NpcManager.AddNpc(Pos, InitAttr);
            Entity.MoveTo(new Vector2(Configure.WindowRight - 100, Random.Range(-100, 100)));
        }
    }

    public class BulletNormalEmitter : BaseEmitter
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

                var Desc = new TrackBulletDescriptor(
                    new BaseBulletDescriptor(Pos, DamageAttr.Get()),
                    ResName, Target, SpeedAttr.Get());

                BulletManager.AddTrackBullet(Desc);
            }
        }
    }
}