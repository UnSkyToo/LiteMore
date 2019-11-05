using System;
using System.Collections.Generic;
using LiteFramework.Helper;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LiteFramework.Extend.LoopView
{
    public abstract class LiteLoopView : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public class ItemEntity
        {
            public readonly LiteLoopViewItem Item;
            public RectTransform ObjRectTrans;
            public int Index;

            public ItemEntity(LiteLoopViewItem Item, int Index)
            {
                this.Item = Item;
                this.ObjRectTrans = Item.Obj.GetOrAddComponent<RectTransform>();
                this.Index = Index;
            }
        }

        public RectTransform Viewport;
        public RectTransform Content;
        public Vector2 ItemSize;
        public float Space;
        public float Elasticity = 0.1f;
        public int TotalCount;

        private bool Inertia_ = true;
        private float DecelerationRate_ = 0.135f;
        private int ViewItemCount_;

        protected Bounds ViewBounds_;
        protected Bounds ContentBounds_;
        private readonly Vector3[] Corners_ = new Vector3[4];
        private Vector2 Velocity_;

        private Vector2 PointerStartLocalCursor_;
        private Vector2 ContentStartPosition_;
        private bool Dragging_;

        private Vector2 PrevPosition_;

        private int CurrentIndex_;
        private int PreviousIndex_;
        private bool IsInit_;

        protected List<ItemEntity> ItemList_;
        protected Func<int, LiteLoopViewItem> OnCreateItem_;
        protected Action<LiteLoopViewItem> OnDeleteItem_;
        protected Action<int, LiteLoopViewItem> OnUpdateItem_;

        protected override void Awake()
        {
            ViewBounds_ = new Bounds(Viewport.rect.center, Viewport.rect.size);
            ViewItemCount_ = InternalCalculateViewItemCount();
            Content.sizeDelta = InternalCalculateContentSize();

            if (TotalCount < ViewItemCount_)
            {
                TotalCount = ViewItemCount_;
            }

            CurrentIndex_ = 0;
            PreviousIndex_ = 0;
            ContentBounds_ = GetBounds();
            ItemList_ = new List<ItemEntity>();
            IsInit_ = false;
        }

        protected override void OnDestroy()
        {
            ClearItemList();
        }

        public void Initialize(int TotalCount, int Index, Func<int, LiteLoopViewItem> CreateItem, Action<LiteLoopViewItem> DeleteItem, Action<int, LiteLoopViewItem> UpdateItem)
        {
            this.TotalCount = TotalCount;
            this.Content.sizeDelta = InternalCalculateContentSize();
            this.CurrentIndex_ = Mathf.Clamp(Index, 0, TotalCount - ViewItemCount_ - 1);
            this.PreviousIndex_ = this.CurrentIndex_;
            this.OnCreateItem_ = CreateItem;
            this.OnDeleteItem_ = DeleteItem;
            this.OnUpdateItem_ = UpdateItem;
            this.Dragging_ = false;
            this.PrevPosition_ = Vector2.zero;
            this.Velocity_ = Vector2.zero;
            this.IsInit_ = true;
            FillItemList();
        }

        public void DeInitialize()
        {
            if (!IsInit_)
            {
                return;
            }

            IsInit_ = false;
            ClearItemList();
        }

        public void SetCurrentIndex(int Index)
        {
            Index = Mathf.Clamp(Index, 0, TotalCount - ViewItemCount_ - 1);
            CurrentIndex_ = Index;
            PreviousIndex_ = Index;
            FillItemList();
        }

        private void ClearItemList()
        {
            for (var Index = 0; Index < ItemList_.Count; ++Index)
            {
                InternalDeleteItem(ItemList_[Index]);
            }
            ItemList_.Clear();
        }

        private void FillItemList()
        {
            ClearItemList();
            for (var Index = 0; Index < ViewItemCount_; ++Index)
            {
                ItemList_.Add(InternalCreateItem(CurrentIndex_ + Index));
                InternalUpdateItem(ItemList_[Index], CurrentIndex_ + Index);
            }
            SetContentAnchoredPosition(InternalCalculatePositionWithIndex(CurrentIndex_));
        }

        private void InternalUpdateItem(ItemEntity Entity, int Index)
        {
            if (Entity.Index == Index)
            {
                return;
            }

            Entity.Index = Index;
            Entity.ObjRectTrans.anchoredPosition = InternalGetItemPosition(Index);
            OnUpdateItem_?.Invoke(Index, Entity.Item);
        }

        private void UpdateItemList()
        {
            var CurIndex = Mathf.Clamp(CurrentIndex_, 0, TotalCount - ViewItemCount_);
            if (PreviousIndex_ == CurIndex)
            {
                return;
            }

            var Count = Mathf.Abs(CurIndex - PreviousIndex_);

            for (var Index = 0; Index < Count; ++Index)
            {
                if (CurIndex > PreviousIndex_)
                {
                    // 0 1 2 3 (+2)
                    //     2 3 4 5
                    var OldIndex = PreviousIndex_ + Index;
                    var NewIndex = PreviousIndex_ + ViewItemCount_ + Index;
                    var Item = ItemList_[0];
                    ItemList_.RemoveAt(0);
                    ItemList_.Add(Item);
                    InternalUpdateItem(Item, NewIndex);
                }
                else
                {
                    //     2 3 4 5 (-2)
                    // 0 1 2 3
                    var OldIndex = PreviousIndex_ + ViewItemCount_ - Index - 1;
                    var NewIndex = PreviousIndex_ - Index - 1;
                    var Item = ItemList_[ViewItemCount_ - 1];
                    ItemList_.RemoveAt(ViewItemCount_ - 1);
                    ItemList_.Insert(0, Item);
                    InternalUpdateItem(Item, NewIndex);
                }
            }

            PreviousIndex_ = CurIndex;
        }

        private Bounds GetBounds()
        {
            Content.GetWorldCorners(Corners_);
            var WorldToLocalMatrix = Viewport.worldToLocalMatrix;
            return InternalGetBounds(Corners_, ref WorldToLocalMatrix);
        }

        private static Bounds InternalGetBounds(Vector3[] Corners, ref Matrix4x4 ViewWorldToLocalMatrix)
        {
            var VecMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var VecMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (var Index = 0; Index < 4; ++Index)
            {
                var Vec = ViewWorldToLocalMatrix.MultiplyPoint3x4(Corners[Index]);
                VecMin = Vector3.Min(Vec, VecMin);
                VecMax = Vector3.Max(Vec, VecMax);
            }

            var Bound = new Bounds(VecMin, Vector3.zero);
            Bound.Encapsulate(VecMax);
            return Bound;
        }

        private void UpdateBounds()
        {
            ViewBounds_ = new Bounds(Viewport.rect.center, Viewport.rect.size);
            ContentBounds_ = GetBounds();

            var Size = ContentBounds_.size;
            var Center = ContentBounds_.center;
            var Pivot = Content.pivot;
            AdjustBounds(ref ViewBounds_, ref Pivot, ref Size, ref Center);
            ContentBounds_.size = Size;
            ContentBounds_.center = Center;
        }

        private static void AdjustBounds(ref Bounds ViewBounds, ref Vector2 ContentPivot, ref Vector3 ContentSize, ref Vector3 ContentPos)
        {
            var Offset = ViewBounds.size - ContentSize;
            if (Offset.x > 0.0f)
            {
                ContentPos.x -= Offset.x * (ContentPivot.x - 0.5f);
                ContentSize.x = ViewBounds.size.x;
            }

            if (Offset.y <= 0.0f)
            {
                return;
            }

            ContentPos.y -= Offset.y * (ContentPivot.y - 0.5f);
            ContentSize.y = ViewBounds.size.y;
        }

        private Vector2 CalculateOffset(Vector2 Delta)
        {
            return InternalCalculateOffset(ref ViewBounds_, ref ContentBounds_, ref Delta);
        }

        protected abstract Vector2 InternalCalculateOffset(ref Bounds ViewBounds, ref Bounds ContentBounds, ref Vector2 Delta);

        private static float RubberDelta(float OverStretching, float ViewSize)
        {
            return (float)(1.0 - 1.0 / ((double)Mathf.Abs(OverStretching) * 0.550000011920929 / (double)ViewSize +
                                        1.0)) * ViewSize * Mathf.Sign(OverStretching);
        }

        private void SetContentAnchoredPosition(Vector2 Position)
        {
            InternalSetContentAnchoredPosition(ref Position);

            if (Position == Content.anchoredPosition)
            {
                return;
            }

            Content.anchoredPosition = Position;
            UpdateBounds();

            CurrentIndex_ = InternalCalculateIndex(Position);
            UpdateItemList();
        }

        public override bool IsActive()
        {
            return base.IsActive() && IsInit_;
        }

        public virtual void OnBeginDrag(PointerEventData EventData)
        {
            if (EventData.button != PointerEventData.InputButton.Left || !IsActive())
            {
                return;
            }

            UpdateBounds();
            PointerStartLocalCursor_ = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Viewport, EventData.position,
                EventData.pressEventCamera, out PointerStartLocalCursor_);
            ContentStartPosition_ = Content.anchoredPosition;
            Dragging_ = true;
        }

        public virtual void OnDrag(PointerEventData EventData)
        {
            var LocalPoint = Vector2.zero;
            if (EventData.button != PointerEventData.InputButton.Left || !IsActive() ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(Viewport, EventData.position,
                    EventData.pressEventCamera, out LocalPoint))
            {
                return;
            }

            UpdateBounds();
            var Vec2 = ContentStartPosition_ + (LocalPoint - PointerStartLocalCursor_);
            var Offset = CalculateOffset(Vec2 - Content.anchoredPosition);
            var Position = Vec2 + Offset;

            if (!Mathf.Approximately(Offset.x, 0.0f))
            {
                Position.x -= RubberDelta(Offset.x, ViewBounds_.size.x);
            }

            if (!Mathf.Approximately(Offset.y, 0.0f))
            {
                Position.y -= RubberDelta(Offset.y, ViewBounds_.size.y);
            }

            SetContentAnchoredPosition(Position);
        }

        public virtual void OnEndDrag(PointerEventData EventData)
        {
            if (EventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Dragging_ = false;
        }

        protected virtual void LateUpdate()
        {
            if (!IsActive())
            {
                return;
            }

            UpdateBounds();

            var UnscaledDeltaTime = Time.unscaledDeltaTime;
            var Offset = CalculateOffset(Vector2.zero);
            if (!Dragging_ && (Offset != Vector2.zero || Velocity_ != Vector2.zero))
            {
                var AnchoredPosition = Content.anchoredPosition;
                for (var Index = 0; Index < 2; ++Index)
                {
                    if (!Mathf.Approximately(Offset[Index], 0.0f))
                    {
                        var CurrentVelocity = Velocity_[Index];
                        AnchoredPosition[Index] = Mathf.SmoothDamp(Content.anchoredPosition[Index],
                            Content.anchoredPosition[Index] + Offset[Index], ref CurrentVelocity,
                            Elasticity, float.PositiveInfinity, UnscaledDeltaTime);
                        if (Mathf.Abs(CurrentVelocity) < 1.0f)
                        {
                            CurrentVelocity = 0.0f;
                        }
                        Velocity_[Index] = CurrentVelocity;
                    }
                    else if (Inertia_)
                    {
                        Velocity_[Index] *= Mathf.Pow(DecelerationRate_, UnscaledDeltaTime);
                        if (Mathf.Abs(Velocity_[Index]) < 1.0f)
                        {
                            Velocity_[Index] = 0.0f;
                        }
                        AnchoredPosition[Index] += Velocity_[Index] * UnscaledDeltaTime;
                    }
                    else
                    {
                        Velocity_[Index] = 0.0f;
                    }
                }

                SetContentAnchoredPosition(AnchoredPosition);
            }

            if (Dragging_ && Inertia_)
            {
                Velocity_ = Vector2.Lerp(Velocity_, (Content.anchoredPosition - PrevPosition_) / UnscaledDeltaTime, UnscaledDeltaTime * 10f);
            }

            PrevPosition_ = Content.anchoredPosition;
        }


        protected abstract int InternalCalculateViewItemCount();
        protected abstract Vector2 InternalCalculateContentSize();
        protected abstract ItemEntity InternalCreateItem(int Index);
        protected abstract void InternalDeleteItem(ItemEntity Entity);
        protected abstract Vector2 InternalGetItemPosition(int Index);
        protected abstract int InternalCalculateIndex(Vector2 Position);
        protected abstract Vector2 InternalCalculatePositionWithIndex(int Index);
        protected abstract void InternalSetContentAnchoredPosition(ref Vector2 Position);
    }
}