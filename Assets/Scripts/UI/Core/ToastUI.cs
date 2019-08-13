using System;
using LiteFramework.Core.Motion;
using LiteFramework.Game.UI;
using LiteMore.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Core
{
    public class ToastUI : BaseUI
    {
        private Action Callback_;

        public ToastUI()
            : base()
        {
            DepthMode = UIDepthMode.Top;
            DepthIndex = 200;
        }

        protected override void OnOpen(params object[] Params)
        {
            var Msg = FindComponent<Text>("Value");
            Msg.text = (string)Params[0];
            Msg.color = (Color)Params[1];
            Callback_ = Params.Length > 2 ? (Action)Params[2] : null;
            var MsgTrans = Msg.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(MsgTrans);
            UIRectTransform.sizeDelta = MsgTrans.sizeDelta + new Vector2(10, 6);

            UITransform.ExecuteMotion(new SequenceMotion(
                new FadeInMotion(0.3f),
                new ScaleMotion(0.1f, new Vector3(1.2f, 1.2f, 1.0f), false),
                new ScaleMotion(0.1f, new Vector3(1.0f, 1.0f, 1.0f), false),
                new WaitTimeMotion(0.5f),
                new ParallelMotion(new MoveMotion(1.0f, new Vector3(0, 100), true),
                    new SequenceMotion(
                        new WaitTimeMotion(0.35f),
                        new CallbackMotion(ToastHelper.ShowAtOnce),
                        new WaitTimeMotion(0.35f),
                        new FadeOutMotion(0.3f))),
                new CallbackMotion(() => { UIManager.CloseUI(this); })));
        }

        protected override void OnClose()
        {
            Callback_?.Invoke();
        }
    }
}