using LiteFramework.Game.UI;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public abstract class DragSelector : BaseSelector
    {
        protected bool IsDrag_;
        protected string DragResName_;
        protected Transform DragObj_;
        protected SpriteRenderer DragObjRender_;

        protected DragSelector(SelectorMode Mode, string DragResName)
            : base(Mode)
        {
            IsDrag_ = false;
            DragResName_ = DragResName;
            DragObj_ = null;
        }

        protected override void OnBindSkill()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.EndDrag, EndDrag);
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.EndDrag, EndDrag);
            DestroyDragObject();
        }

        public override void Recreated()
        {
            DestroyDragObject();
            CreateDragObject();
        }

        private void CreateDragObject()
        {
            if (DragObj_ != null)
            {
                return;
            }

            DragObj_ = Object.Instantiate(Resources.Load<GameObject>(DragResName_)).transform;
            DragObj_.SetParent(Configure.SfxRoot, false);
            DragObj_.localPosition = Vector3.zero;
            DragObjRender_ = DragObj_.GetComponent<SpriteRenderer>();
            DragObjRender_.color = Color.green;
            DragObjRender_.size = new Vector2(Skill_.Radius * 2, Skill_.Radius * 2);
        }

        private void DestroyDragObject()
        {
            if (DragObj_ == null)
            {
                return;
            }

            Object.Destroy(DragObj_.gameObject);
            DragObj_ = null;
        }

        private void BeginDrag(GameObject Sender, Vector2 Pos)
        {
            if (!Skill_.CanUse())
            {
                return;
            }

            IsDrag_ = true;
            CreateDragObject();

            Configure.SkillCancel.gameObject.SetActive(true);
            OnBeginDrag(Pos);
        }

        protected abstract void OnBeginDrag(Vector2 Pos);

        private void Drag(GameObject Sender, Vector2 Pos)
        {
            if (!IsDrag_)
            {
                return;
            }

            if (Configure.SkillCancelRect.Contains(Pos))
            {
                DragObjRender_.color = Color.red;
            }
            else
            {
                DragObjRender_.color = Color.green;
            }

            OnDrag(Pos);
        }

        protected abstract void OnDrag(Vector2 Pos);

        private void EndDrag(GameObject Sender, Vector2 Pos)
        {
            IsDrag_ = false;
            OnEndDrag(Pos);
            Configure.SkillCancel.gameObject.SetActive(false);
            DestroyDragObject();

            if (Configure.SkillCancelRect.Contains(Pos))
            {
                return;
            }

            if (!Skill_.CanUse())
            {
                return;
            }

            OnDragSpell(Pos);
        }

        protected abstract void OnEndDrag(Vector2 Pos);

        protected abstract void OnDragSpell(Vector2 Pos);
    }
}