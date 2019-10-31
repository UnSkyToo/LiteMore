using System;
using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteFramework.Game.UI;
using LiteMore.UI.Core;
using LiteMore.UI.Logic;

namespace LiteMore.UI
{
    public static class UIConfigure
    {
        public static readonly Dictionary<Type, UIDescriptor> UIList = new Dictionary<Type, UIDescriptor>
        {
            {typeof(ToastUI), new UIDescriptor(new AssetUri("UI/ToastUI.prefab"), true, false)},
            {typeof(TipsUI), new UIDescriptor(new AssetUri("UI/TipsUI.prefab"), false, false)},
            {typeof(GameOverUI), new UIDescriptor(new AssetUri("UI/GameOverUI.prefab"), false, true)},
            {typeof(MainUI), new UIDescriptor(new AssetUri("UI/MainUI.prefab"), false, true)},
            {typeof(DpsUI), new UIDescriptor(new AssetUri("UI/DpsUI.prefab"), false, false)},
            {typeof(QuickControlUI), new UIDescriptor(new AssetUri("UI/QuickControlUI.prefab"), false, true)},
            {typeof(JoystickUI), new UIDescriptor(new AssetUri("UI/JoystickUI.prefab"), false, false)},
            {typeof(MainOperatorUI), new UIDescriptor(new AssetUri("UI/MainOperatorUI.prefab"), false, true)},
            {typeof(HeroListUI), new UIDescriptor(new AssetUri("UI/HeroListUI.prefab"), false, true)},
        };
    }
}