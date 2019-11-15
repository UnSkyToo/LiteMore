using LiteFramework.Game.Asset;
using LiteFramework.Game.EventSystem;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
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
            var CancelPos = UIHelper.WorldPosToScreenPos(DragCancelObj_.position);
            DragCancelRect_ = new Rect(CancelPos - DragCancelObj_.sizeDelta / 2, DragCancelObj_.sizeDelta);
            EventHelper.AddEvent(Carrier_, BeginDrag, EventSystemType.BeginDrag);
            EventHelper.AddEvent(Carrier_, Drag, EventSystemType.Drag);
            EventHelper.AddEvent(Carrier_, EndDrag, EventSystemType.EndDrag);
        }

        public override void Dispose()
        {
            EventHelper.RemoveEvent(Carrier_, BeginDrag, EventSystemType.BeginDrag);
            EventHelper.RemoveEvent(Carrier_, Drag, EventSystemType.Drag);
            EventHelper.RemoveEvent(Carrier_, EndDrag, EventSystemType.EndDrag);
            DestroyDragObject();
        }

        private void CreateDragObject()
        {
            if (DragObj_ != null)
            {
                return;
            }

            DragObj_ = AssetManager.CreatePrefabSync(new AssetUri(DragResName_)).transform;
            MapManager.AddToGroundLayer(DragObj_);
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

        private void BeginDrag(EventSystemData Data)
        {
            if (!CanUse())
            {
                return;
            }

            IsDrag_ = true;
            CreateDragObject();

            DragCancelObj_.gameObject.SetActive(true);
            OnBeginDrag(UIHelper.ScreenPosToCanvasPos(Configure.CanvasRoot, Data.Location));
        }

        protected abstract void OnBeginDrag(Vector2 Pos);

        private void Drag(EventSystemData Data)
        {
            if (!IsDrag_)
            {
                return;
            }

            if (DragCancelRect_.Contains(Data.Location))
            {
                DragObjRender_.color = Color.red;
            }
            else
            {
                DragObjRender_.color = Color.green;
            }

            OnDrag(UIHelper.ScreenPosToCanvasPos(Configure.CanvasRoot, Data.Location));
        }

        protected abstract void OnDrag(Vector2 Pos);

        private void EndDrag(EventSystemData Data)
        {
            IsDrag_ = false;
            OnEndDrag(UIHelper.ScreenPosToCanvasPos(Configure.CanvasRoot, Data.Location));
            DragCancelObj_.gameObject.SetActive(false);
            DestroyDragObject();

            if (DragCancelRect_.Contains(Data.Location))
            {
                return;
            }

            if (!CanUse())
            {
                return;
            }

            OnDragSpell(UIHelper.ScreenPosToCanvasPos(Configure.CanvasRoot, Data.Location));
        }

        protected abstract void OnEndDrag(Vector2 Pos);

        protected abstract void OnDragSpell(Vector2 Pos);
    }
}