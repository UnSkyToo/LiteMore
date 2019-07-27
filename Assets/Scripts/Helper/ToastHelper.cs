using System.Collections.Generic;
using LiteMore.UI;
using LiteMore.UI.Core;
using UnityEngine;

namespace LiteMore.Helper
{
    public static class ToastHelper
    {
        private struct ToastInfo
        {
            internal string Msg { get; }
            internal Color MsgColor { get; }

            public ToastInfo(string Msg, Color MsgColor)
            {
                this.Msg = Msg;
                this.MsgColor = MsgColor;
            }
        }

        private static readonly Queue<ToastInfo> ToastList_ = new Queue<ToastInfo>();

        public static void Show(string Msg)
        {
            Show(Msg, Color.white);
        }

        public static void Show(string Msg, Color MsgColor)
        {
            if (!UIManager.IsClosed<ToastUI>())
            {
                ToastList_.Enqueue(new ToastInfo(Msg, MsgColor));
                return;
            }

            UIManager.OpenUI<ToastUI>(Msg, MsgColor);
        }

        public static void ShowAtOnce()
        {
            if (ToastList_.Count > 0)
            {
                var Info = ToastList_.Dequeue();
                UIManager.OpenUI<ToastUI>(Info.Msg, Info.MsgColor);
            }
        }
    }
}