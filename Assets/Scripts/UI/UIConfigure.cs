using System;
using System.Collections.Generic;
using LiteFramework.Game.UI;
using LiteMore.UI.Core;
using LiteMore.UI.Logic;

namespace LiteMore.UI
{
    public static class UIConfigure
    {
        public static readonly Dictionary<Type, UIDescriptor> UIList = new Dictionary<Type, UIDescriptor>
        {
            {typeof(ToastUI), new UIDescriptor("UI/ToastUI", true, false)},
            {typeof(TipsUI), new UIDescriptor("UI/TipsUI", false, false)},
            {typeof(GameOverUI), new UIDescriptor("UI/GameOverUI", false, true)},
            {typeof(MainUI), new UIDescriptor("UI/MainUI", false, true)},
            {typeof(DpsUI), new UIDescriptor("UI/DpsUI", false, false)},
            {typeof(QuickControlUI), new UIDescriptor("UI/QuickControlUI", false, true)},
        };
    }
}