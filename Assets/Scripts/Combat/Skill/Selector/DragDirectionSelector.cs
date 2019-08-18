using System.Collections.Generic;
using LiteFramework.Helper;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragDirectionSelector : DragSelector
    {
        private Vector2 OriginPos_;

        public DragDirectionSelector(string DragResName)
            : base(SelectorMode.DragDirection, DragResName)
        {
        }

        protected override void OnBindCarrier(Dictionary<string, object> Args)
        {
            base.OnBindCarrier(Args);
            OriginPos_ = (Vector2)Args["OriginPos"];
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
            Used(new Dictionary<string, object>
            {
                {"Direction", (Pos - OriginPos_).normalized},
            });
        }
    }
}