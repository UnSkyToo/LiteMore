using LiteFramework;
using LiteFramework.Game.UI;

namespace LiteMore.UI.Logic
{
    public class GameOverUI : BaseUI
    {
        public GameOverUI()
            : base()
        {
            DepthMode = UIDepthMode.Top;
            DepthIndex = 1000;
        }

        protected override void OnOpen(params object[] Params)
        {
            AddEventToChild("BtnRestart", (Obj, Pos) =>
            {
                LiteManager.Restart();
            });
        }
    }
}