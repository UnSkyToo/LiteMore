using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public class DragPositionSelector : DragSelector
    {
        public event System.Action<DragSelectorDescriptor, Vector2> OnUsed;

        public DragPositionSelector(DragSelectorDescriptor Desc)
            : base(SelectorMode.DragPosition, Desc)
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
            OnUsed?.Invoke(Desc_, Pos);
        }
    }
}