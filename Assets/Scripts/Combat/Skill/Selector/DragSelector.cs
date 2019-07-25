using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public struct DragSelectorDescriptor
    {
        public MainSkill Skill { get; }
        public string ResName { get; }

        public DragSelectorDescriptor(MainSkill Skill, string ResName)
        {
            this.Skill = Skill;
            this.ResName = ResName;
        }
    }

    public abstract class DragSelector : BaseSelector
    {
        protected DragSelectorDescriptor Desc_;
        protected bool IsDrag_;
        protected Transform DragObj_;
        protected SpriteRenderer DragObjRender_;

        protected DragSelector(SelectorMode Mode, DragSelectorDescriptor Desc)
            : base(Mode, Desc.Skill)
        {
            Desc_ = Desc;

            IsDrag_ = false;
            DragObj_ = null;
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Skill_.IconTransform).AddCallback(UIEventType.EndDrag, EndDrag);
        }

        public override void Destroy()
        {
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Skill_.IconTransform).RemoveCallback(UIEventType.EndDrag, EndDrag);
            DestroyDragObject();
        }

        private void CreateDragObject()
        {
            if (DragObj_ != null)
            {
                return;
            }

            DragObj_ = Object.Instantiate(Resources.Load<GameObject>(Desc_.ResName)).transform;
            DragObj_.SetParent(Configure.SfxRoot, false);
            DragObj_.localPosition = Vector3.zero;
            DragObjRender_ = DragObj_.GetComponent<SpriteRenderer>();
            DragObjRender_.color = Color.green;
            DragObjRender_.size = new Vector2(Desc_.Skill.Radius * 2, Desc_.Skill.Radius * 2);
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

            if (!Skill_.Use())
            {
                return;
            }

            OnDragSpell(Pos);
        }

        protected abstract void OnEndDrag(Vector2 Pos);

        protected abstract void OnDragSpell(Vector2 Pos);
    }
}