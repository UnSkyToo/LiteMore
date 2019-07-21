using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class TipsUI : UIBase
    {
        private Text Text_;
        private RectTransform TextRectTransform_;

        public TipsUI()
            : base()
        {
            DepthMode = UIDepthMode.Top;
            DepthIndex = 100;
        }

        protected override void OnOpen(params object[] Params)
        {
            Text_ = FindComponent<Text>("Text");
            Text_.text = string.Empty;

            TextRectTransform_ = Text_.gameObject.GetComponent<RectTransform>();

            FlushData((string)Params[0], (Vector2)Params[1]);
        }

        private void FlushData(string Msg, Vector2 Position)
        {
            // TODO: 第二次赋值无法计算正确的size，采用不缓存UI的方案

            Text_.text = Msg;
            LayoutRebuilder.ForceRebuildLayoutImmediate(TextRectTransform_);
            UIRectTransform.sizeDelta = new Vector2(8 * 2 + Text_.preferredWidth, 8 * 2 + Text_.preferredHeight);
            LayoutRebuilder.ForceRebuildLayoutImmediate(UIRectTransform);

            Position -= (UIRectTransform.sizeDelta / 2 + new Vector2(5, 5));

            var X = Mathf.Clamp(Position.x,
                Configure.WindowLeft + UIRectTransform.sizeDelta.x / 2,
                Configure.WindowRight - UIRectTransform.sizeDelta.x / 2);
            var Y = Mathf.Clamp(Position.y,
                Configure.WindowBottom + UIRectTransform.sizeDelta.y / 2,
                Configure.WindowTop - UIRectTransform.sizeDelta.y / 2);

            UIRectTransform.anchoredPosition = new Vector2(X, Y);
        }
    }
}