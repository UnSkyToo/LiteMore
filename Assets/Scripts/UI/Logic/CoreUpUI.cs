namespace LiteMore.UI.Logic
{
    public class CoreUpUI : BaseUI
    {
        public CoreUpUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 100;
        }

        protected override void OnOpen(params object[] Params)
        {
            AddEvent((Obj, Pos) =>
            {
                UIManager.CloseUI(this);
            });
        }
    }
}