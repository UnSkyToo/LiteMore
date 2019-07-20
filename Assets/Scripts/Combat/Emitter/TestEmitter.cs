using LiteMore.Combat.Bullet;
using LiteMore.Combat.Npc;
using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Emitter
{
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

                var Entity = BulletManager.AddTrackBullet(ResName, Pos);
                Entity.Speed = SpeedAttr.Get();
                Entity.Damage = DamageAttr.Get();
                Entity.Attack(Target);
            }
        }
    }
}