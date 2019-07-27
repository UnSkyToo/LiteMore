using System;
using System.Collections.Generic;
using LiteMore.UI.Core;
using LiteMore.UI.Logic;

namespace LiteMore.UI
{
    public class UIDescriptor
    {
        public string PrefabName { get; }
        public bool OpenMore { get; }
        public bool Cached { get; }

        public UIDescriptor(string PrefabName)
            : this(PrefabName, true, true)
        {
        }

        public UIDescriptor(string PrefabName, bool OpenMore, bool Cached)
        {
            this.PrefabName = PrefabName;
            this.OpenMore = OpenMore;
            this.Cached = Cached;
        }
    }

    public static class UIConfigure
    {
        public static readonly Dictionary<Type, UIDescriptor> UIList = new Dictionary<Type, UIDescriptor>
        {
            {typeof(ToastUI), new UIDescriptor("ToastUI", true, false)},
            {typeof(TipsUI), new UIDescriptor("TipsUI", false, false)},
            {typeof(GameOverUI), new UIDescriptor("GameOverUI", false, true)},
            {typeof(MainUI), new UIDescriptor("MainUI", false, true)},
            {typeof(CoreUpUI), new UIDescriptor("CoreUpUI", false, true)},
        };
    }
}