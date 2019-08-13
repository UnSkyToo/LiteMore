using System.Collections.Generic;
using LiteFramework.Helper;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragDirectionSelector : DragSelector
    {
        private readonly Vector2 OriginPos_;

        public DragDirectionSelector(string DragResName, Vector2 OriginPos)
            : base(SelectorMode.DragDirection, DragResName)
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
            Skill_.Use(new Dictionary<string, object>
            {
                {"Direction", (Pos - OriginPos_).normalized},
            });
        }
    }
}