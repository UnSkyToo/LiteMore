using System;
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
            internal Action Callback { get; }

            public ToastInfo(string Msg, Color MsgColor, Action Callback)
            {
                this.Msg = Msg;
                this.MsgColor = MsgColor;
                this.Callback = Callback;
            }
        }

        private static readonly Queue<ToastInfo> ToastList_ = new Queue<ToastInfo>();

        public static void Show(string Msg)
        {
            Show(Msg, Color.white);
        }

        public static void Show(string Msg, Color MsgColor, Action Callback = null)
        {
            if (!UIManager.IsClosed<ToastUI>())
            {
                ToastList_.Enqueue(new ToastInfo(Msg, MsgColor, Callback));
                return;
            }

            UIManager.OpenUI<ToastUI>(Msg, MsgColor, Callback);
        }

        public static void ShowAtOnce()
        {
            if (ToastList_.Count > 0)
            {
                var Info = ToastList_.Dequeue();
                UIManager.OpenUI<ToastUI>(Info.Msg, Info.MsgColor, Info.Callback);
            }
        }
    }
}