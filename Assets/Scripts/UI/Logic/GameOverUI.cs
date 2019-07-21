﻿namespace LiteMore.UI.Logic
{
    public class GameOverUI : UIBase
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
                GameManager.Restart();
            });
        }
    }
}