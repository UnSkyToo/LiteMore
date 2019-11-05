#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Extend.LoopView
{
    [AddComponentMenu("UI/Lite/Vertical LoopView")]
    public class LiteVerticalLoopView : LiteLoopView
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Lite/Vertical LoopView")]
        private static void CreateLoopView()
        {
            var Root = new GameObject("LoopViewV").AddComponent<RectTransform>();
            Root.transform.SetParent(Selection.activeTransform, false);

            var Viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            Viewport.SetParent(Root.transform, false);
            Viewport.anchorMin = Vector2.zero;
            Viewport.anchorMax = Vector2.one;
            Viewport.pivot = new Vector2(0, 1);
            Viewport.rotation = Quaternion.identity;
            Viewport.localScale = Vector3.one;
            Viewport.anchoredPosition = Vector2.zero;
            Viewport.sizeDelta = Vector2.zero;
            var ViewportMaskImage = Viewport.gameObject.AddComponent<Image>();
            ViewportMaskImage.sprite = Resources.Load<Sprite>("UIMask");
            ViewportMaskImage.type = Image.Type.Sliced;
            ViewportMaskImage.fillCenter = true;
            ViewportMaskImage.raycastTarget = true;
            var ViewportMask = Viewport.gameObject.AddComponent<Mask>();
            ViewportMask.showMaskGraphic = false;

            var Content = new GameObject("Content").AddComponent<RectTransform>();
            Content.SetParent(Viewport, false);
            Content.anchorMin = new Vector2(0, 1);
            Content.anchorMax = Vector2.one;
            Content.pivot = new Vector2(0, 1);
            Content.rotation = Quaternion.identity;
            Content.localScale = Vector3.one;
            Content.anchoredPosition = Vector2.zero;
            Content.sizeDelta = Vector2.zero;

            var Script = Root.gameObject.AddComponent<LiteVerticalLoopView>();
            Script.Viewport = Viewport;
            Script.Content = Content;
            Script.TotalCount = 1;

            Selection.activeGameObject = Root.gameObject;
        }
#endif

        protected override int InternalCalculateViewItemCount()
        {
            return Mathf.RoundToInt(transform.GetComponent<RectTransform>().sizeDelta.y / ItemSize.y) + 1;
        }

        protected override Vector2 InternalCalculateContentSize()
        {
            return new Vector2(0, TotalCount * (ItemSize.y + Space));
        }

        protected override ItemEntity InternalCreateItem(int Index)
        {
            var Item = OnCreateItem_?.Invoke(Index);
            Item.Obj.transform.SetParent(Content, false);
            Item.Obj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            Item.Obj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            return new ItemEntity(Item, -1);
        }

        protected override void InternalDeleteItem(ItemEntity Entity)
        {
            OnDeleteItem_?.Invoke(Entity.Item);
        }

        protected override Vector2 InternalGetItemPosition(int Index)
        {
            return new Vector2(0, -Index * (ItemSize.y + Space) - (ItemSize.y * 0.5f));
        }

        protected override int InternalCalculateIndex(Vector2 Position)
        {
            return Mathf.FloorToInt(Position.y / (ItemSize.y + Space));
        }

        protected override Vector2 InternalCalculatePositionWithIndex(int Index)
        {
            return new Vector2(0, Index * (ItemSize.y + Space));
        }

        protected override Vector2 InternalCalculateOffset(ref Bounds ViewBounds, ref Bounds ContentBounds, ref Vector2 Delta)
        {
            var Zero = Vector2.zero;
            var Min = (Vector2)ContentBounds.min;
            var Max = (Vector2)ContentBounds.max;

            Min.y += Delta.y;
            Max.y += Delta.y;
            if (Max.y < ViewBounds.max.y)
            {
                Zero.y = ViewBounds.max.y - Max.y;
            }
            else if (Min.y > ViewBounds.min.y)
            {
                Zero.y = ViewBounds.min.y - Min.y;
            }

            return Zero;
        }

        protected override void InternalSetContentAnchoredPosition(ref Vector2 Position)
        {
            Position.x = Content.anchoredPosition.x;
        }
    }
}