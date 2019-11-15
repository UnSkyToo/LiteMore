using LiteFramework;
using LiteFramework.Game.UI;

namespace LiteMore.UI.Logic
{
    public class GameOverUI : BaseUI
    {
        public GameOverUI()
            : base(UIDepthMode.Top, 1)
        {
        }

        protected override void OnOpen(params object[] Params)
        {
            AddEventToChild("BtnRestart", LiteManager.Restart);
        }
    }
}