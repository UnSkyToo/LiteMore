using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragDirectionSelector : DragSelector
    {
        public event System.Action<DragSelectorDescriptor, Vector2> OnUsed;
        private readonly Vector2 OriginPos_;

        public DragDirectionSelector(DragSelectorDescriptor Desc, Vector2 OriginPos)
            : base(SelectorMode.DragDirection, Desc)
        {
            OriginPos_ = OriginPos;
        }

        protected override void OnBeginDrag(Vector2 Pos)
        {
        }

        protected override void OnDrag(Vector2 Pos)
        {
            var Angle = MathHelper.GetUnityAngle(OriginPos_, Pos);
            DragObj_.localPosition = OriginPos_;
            DragObj_.localRotation = Quaternion.AngleAxis(Angle, Vector3.forward);
        }

        protected override void OnEndDrag(Vector2 Pos)
        {
        }

        protected override void OnDragSpell(Vector2 Pos)
        {
            OnUsed?.Invoke(Desc_, (Pos - OriginPos_).normalized);
        }
    }
}