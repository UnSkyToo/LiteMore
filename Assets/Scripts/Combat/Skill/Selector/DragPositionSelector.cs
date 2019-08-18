using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragPositionSelector : DragSelector
    {
        public DragPositionSelector(string DragResName)
            : base(SelectorMode.DragPosition, DragResName)
        {
        }

        protected override void OnBeginDrag(Vector2 Pos)
        {
        }

        protected override void OnDrag(Vector2 Pos)
        {
            DragObj_.localPosition = Pos;
        }

        protected override void OnEndDrag(Vector2 Pos)
        {
        }

        protected override void OnDragSpell(Vector2 Pos)
        {
            Used(new Dictionary<string, object>
            {
                {"Position", Pos},
                {"Radius", DragRadius_},
            });
        }
    }
}