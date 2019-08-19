using LiteFramework.Game.Asset;
using LiteFramework.Game.UI;
using UnityEngine;

namespace LiteMore.Combat.Skill.Selector
{
    public abstract class DragSelector : BaseSelector
    {
        protected bool IsDrag_;
        protected string DragResName_;
        protected float DragRadius_;
        protected RectTransform DragCancelObj_;
        protected Rect DragCancelRect_;
        protected Transform DragObj_;
        protected SpriteRenderer DragObjRender_;

        protected DragSelector(SelectorMode Mode, string DragResName)
            : base(Mode)
        {
            IsDrag_ = false;
            DragResName_ = DragResName;
            DragObj_ = null;
        }

        protected override void OnBindCarrier()
        {
            DragRadius_ = Args_.Skill.Radius;
            DragCancelObj_ = Args_.CancelObj;
            DragCancelRect_ = new Rect(new Vector2(DragCancelObj_.position.x, DragCancelObj_.position.y) - DragCancelObj_.sizeDelta / 2, DragCancelObj_.sizeDelta);
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Carrier_).AddCallback(UIEventType.EndDrag, EndDrag);
        }

        public override void Dispose()
        {
            UIEventTriggerListener.Get(Carrier_).RemoveCallback(UIEventType.BeginDrag, BeginDrag);
            UIEventTriggerListener.Get(Carrier_).RemoveCallback(UIEventType.Drag, Drag);
            UIEventTriggerListener.Get(Carrier_).RemoveCallback(UIEventType.EndDrag, EndDrag);
            DestroyDragObject();
        }

        private void CreateDragObject()
        {
            if (DragObj_ != null)
            {
                return;
            }

            DragObj_ = AssetManager.CreatePrefabSync(DragResName_).transform;
            DragObj_.SetParent(Configure.SfxRoot, false);
            DragObj_.localPosition = Vector3.zero;
            DragObjRender_ = DragObj_.GetComponent<SpriteRenderer>();
            DragObjRender_.color = Color.green;
            DragObjRender_.size = new Vector2(DragRadius_ * 2, DragRadius_ * 2);
        }

        private void DestroyDragObject()
        {
            if (DragObj_ == null)
            {
                return;
            }

            AssetManager.DeleteAsset(DragObj_.gameObject);
            DragObj_ = null;
        }

        private void BeginDrag(GameObject Sender, Vector2 Pos)
        {
            if (!CanUse())
            {
                return;
            }

            IsDrag_ = true;
            CreateDragObject();

            DragCancelObj_.gameObject.SetActive(true);
            OnBeginDrag(Pos);
        }

        protected abstract void OnBeginDrag(Vector2 Pos);

        private void Drag(GameObject Sender, Vector2 Pos)
        {
            if (!IsDrag_)
            {
                return;
            }

            if (DragCancelRect_.Contains(Pos))
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
            DragCancelObj_.gameObject.SetActive(false);
            DestroyDragObject();

            if (DragCancelRect_.Contains(Pos))
            {
                return;
            }

            if (!CanUse())
            {
                return;
            }

            OnDragSpell(Pos);
        }

        protected abstract void OnEndDrag(Vector2 Pos);

        protected abstract void OnDragSpell(Vector2 Pos);
    }
}