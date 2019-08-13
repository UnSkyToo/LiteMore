using LiteFramework.Extend;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Emitter
{
    public abstract class NpcEmitter : BaseEmitter
    {
        public EmitterRandFloat SpeedAttr { get; set; }
        public EmitterRandFloat HpAttr { get; set; }
        public EmitterRandFloat DamageAttr { get; set; }
        public EmitterRandInt GemAttr { get; set; }

        protected NpcEmitter(string Name)
            : base(Name)
        {
        }
    }

    public class NpcRectEmitter : NpcEmitter
    {
        public EmitterRandVector2 OffsetAttr { get; set; }

        private Transform ObjOuter_;
        private LineCaller CallerOuter_;

        public NpcRectEmitter(string Name)
            : base(Name)
        {
        }

        public override void CreateDebugLine()
        {
            ObjOuter_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line")).transform;
            ObjOuter_.name = $"NpcRectOuter<{ID}>";
            ObjOuter_.SetParent(Configure.EmitterRoot, false);
            ObjOuter_.localPosition = Vector3.zero;

            CallerOuter_ = new LineCaller(ObjOuter_.GetComponent<LineRenderer>());
            CallerOuter_.DrawRect(
                new LineCallerPoint(Position + OffsetAttr.Min, Color.blue),
                new LineCallerPoint(Position + OffsetAttr.Max, Color.blue));
        }

        public override void Dispose()
        {
            Object.Destroy(ObjOuter_.gameObject);
        }

        protected override void OnEmitted(uint Cur, uint Max)
        {
            var Pos = Position + OffsetAttr.Get();
            var InitAttr = NpcManager.GenerateInitAttr(SpeedAttr.Get(), HpAttr.Get(), 0, DamageAttr.Get(), GemAttr.Get(), 5, 5);
            var Entity = NpcManager.AddNpc(Name, Pos, Team, InitAttr);
            //Entity.MoveTo(Configure.CoreBasePosition);
        }
    }

    public class NpcCircleEmitter : NpcEmitter
    {
        public EmitterRandFloat RadiusAttr { get; set; }

        private Transform ObjInner_;
        private LineCaller CallerInner_;

        private Transform ObjOuter_;
        private LineCaller CallerOuter_;

        public NpcCircleEmitter(string Name)
            : base(Name)
        {
        }

        public override void CreateDebugLine()
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

        public override void Dispose()
        {
            Object.Destroy(ObjInner_.gameObject);
            Object.Destroy(ObjOuter_.gameObject);
        }

        protected override void OnEmitted(uint Cur, uint Max)
        {
            var Radius = RadiusAttr.Get();
            var Angle = Random.Range(0, Mathf.PI * 2);
            var Pos = Position + new Vector2(Mathf.Sin(Angle) * Radius, Mathf.Cos(Angle) * Radius);

            var InitAttr = NpcManager.GenerateInitAttr(SpeedAttr.Get(), HpAttr.Get(), 0, DamageAttr.Get(), GemAttr.Get(), 5, 5);
            var Entity = NpcManager.AddNpc(Name, Pos, Team, InitAttr);
            //Entity.MoveTo(Configure.CoreBasePosition);
        }
    }
}