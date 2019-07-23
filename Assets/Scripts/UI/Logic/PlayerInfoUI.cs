using LiteMore.Helper;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class PlayerInfoUI : BaseUI
    {
        private Text HpText_;
        private Text MpText_;
        private Text GemText_;

        public PlayerInfoUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;
        }

        protected override void OnOpen(params object[] Params)
        {
            HpText_ = FindComponent<Text>("BG/Hp");
            MpText_ = FindComponent<Text>("BG/Mp");
            GemText_ = FindComponent<Text>("BG/Gem");

            OnPlayerHpChangeEvent(null);
            OnPlayerMpChangeEvent(null);
            OnPlayerGemChangeEvent(null);

            LayoutRebuilder.ForceRebuildLayoutImmediate(UIRectTransform);

            var TipsMsg = $"<color=#ff0000><size=30>生命值归零游戏结束</size></color>\n" +
                          "<color=#00ff00><size=30>魔法值用于释放技能</size></color>\n" +
                          "<color=#ffff00><size=30>宝石是通用货币</size></color>";

            UIHelper.AddTips(UITransform, "BG", ()=> TipsMsg);

            EventManager.Register<PlayerHpChangeEvent>(OnPlayerHpChangeEvent);
            EventManager.Register<PlayerMpChangeEvent>(OnPlayerMpChangeEvent);
            EventManager.Register<PlayerGemChangeEvent>(OnPlayerGemChangeEvent);
        }

        protected override void OnClose()
        {
            EventManager.UnRegister<PlayerHpChangeEvent>(OnPlayerHpChangeEvent);
            EventManager.UnRegister<PlayerMpChangeEvent>(OnPlayerMpChangeEvent);
            EventManager.UnRegister<PlayerGemChangeEvent>(OnPlayerGemChangeEvent);
        }

        private void OnPlayerHpChangeEvent(PlayerHpChangeEvent Event)
        {
            HpText_.text = $"{"Hp".Localize()}:{(int)PlayerManager.Hp}/{(int)PlayerManager.MaxHp}(+{PlayerManager.HpAdd}/s)";
        }

        private void OnPlayerMpChangeEvent(PlayerMpChangeEvent Event)
        {
            MpText_.text = $"{"Mp".Localize()}:{(int)PlayerManager.Mp}/{(int)PlayerManager.MaxMp}(+{PlayerManager.MpAdd}/s)";
        }

        private void OnPlayerGemChangeEvent(PlayerGemChangeEvent Event)
        {
            GemText_.text = $"{"Gem".Localize()}:{PlayerManager.Gem}";
        }
    }
}