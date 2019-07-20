using System;
using System.Collections.Generic;
using LiteMore.UI.Logic;

namespace LiteMore.UI
{
    public class UIDescriptor
    {
        public string PrefabName { get; }
        public bool OpenMore { get; }

        public UIDescriptor(string PrefabName)
            : this(PrefabName, true)
        {
        }

        public UIDescriptor(string PrefabName, bool OpenMore)
        {
            this.PrefabName = PrefabName;
            this.OpenMore = OpenMore;
        }
    }

    public static class UIConfigure
    {
        public static readonly Dictionary<Type, UIDescriptor> UIList = new Dictionary<Type, UIDescriptor>
        {
            {typeof(TipsUI), new UIDescriptor("TipsUI", false)},
            {typeof(GameOverUI), new UIDescriptor("GameOverUI", false)},
            {typeof(PlayerInfoUI), new UIDescriptor("PlayerInfoUI", false)},
        };
    }
}