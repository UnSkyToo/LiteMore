using LiteFramework.Helper;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragDirectionSelector : DragSelector
    {
        private BaseNpc Master_;
        private Vector2 DragBeginPos_;

        public DragDirectionSelector(string DragResName)
            : base(SelectorMode.DragDirection, DragResName)
        {
        }

        public override BaseSelector Clone()
        {
            return new DragDirectionSelector(DragResName_);
        }

        protected override void OnBindCarrier()
        {
            base.OnBindCarrier();
            Master_ = Args_.Skill.Master;
        }

        public override void Tick(float DeltaTime)
        {
            if (IsDrag_)
            {
                DragObj_.localPosition = Master_.Position;
            }
        }

        protected override void OnBeginDrag(Vector2 Pos)
        {
            DragBeginPos_ = Pos;
        }

        protected override void OnDrag(Vector2 Pos)
        {
            var Angle = MathHelper.GetUnityAngle(DragBeginPos_, Pos);
            DragObj_.localRotation = Quaternion.AngleAxis(Angle, Vector3.forward);
        }

        protected override void OnEndDrag(Vector2 Pos)
        {
        }

        protected override void OnDragSpell(Vector2 Pos)
        {
            Args_.Direction = (Pos - DragBeginPos_).normalized;
            Used();
        }
    }
}