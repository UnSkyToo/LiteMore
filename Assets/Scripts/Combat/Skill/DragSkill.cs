using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Skill
{
    public abstract class DragSkill : BaseSkill
    {
        protected bool IsDrag_;

        protected DragSkill(Transform Trans, SkillDescriptor Desc)
            : base(Trans, Desc)
        {
            IsDrag_ = false;
            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.EndDrag, EndDrag);
        }

        private void BeginDrag(GameObject Sender, Vector2 Pos)
        {
            if (PlayerManager.Mp < Desc_.Cost || IsCD_)
            {
                return;
            }

            IsDrag_ = true;
            OnBeginDrag(Pos);
        }

        protected abstract void OnBeginDrag(Vector2 Pos);

        private void Drag(GameObject Sender, Vector2 Pos)
        {
            if (!IsDrag_)
            {
                return;
            }

            OnDrag(Pos);
        }

        protected abstract void OnDrag(Vector2 Pos);

        private void EndDrag(GameObject Sender, Vector2 Pos)
        {
            if (PlayerManager.Mp < Desc_.Cost)
            {
                return;
            }

            IsDrag_ = false;
            OnEndDrag(Pos);
        }

        protected abstract void OnEndDrag(Vector2 Pos);
    }
}