using LiteMore.Combat.Wave;
using LiteMore.Helper;
using LiteMore.Motion;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class MainUI : BaseUI
    {
        private RectTransform CoreInfoTrans_;
        private Text HpText_;
        private Text MpText_;
        private Text GemText_;

        private RectTransform WaveInfoTrans_;
        private Text CurrentWaveText_;
        private Text RemainingNumText_;
        private Text RewardText_;
        private Text NewWaveText_;

        public MainUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;
        }

        protected override void OnOpen(params object[] Params)
        {
            InitCoreInfo();
            RefreshCoreInfo();

            InitWaveInfo();
            RefreshWaveInfo();
        }

        protected override void OnClose()
        {
            EventManager.UnRegister<CoreInfoChangeEvent>(OnCoreInfoChangeEvent);
            EventManager.UnRegister<WaveChangeEvent>(OnWaveChangeEvent);
            EventManager.UnRegister<NewWaveEvent>(OnNewWaveEvent);
        }

        private void InitCoreInfo()
        {
            CoreInfoTrans_ = FindComponent<RectTransform>("CoreInfo");
            HpText_ = FindComponent<Text>("CoreInfo/Hp");
            MpText_ = FindComponent<Text>("CoreInfo/Mp");
            GemText_ = FindComponent<Text>("CoreInfo/Gem");

            var TipsMsg = $"<color=#ff0000><size=30>生命值归零游戏结束</size></color>\n" +
                          "<color=#00ff00><size=30>魔法值用于释放技能</size></color>\n" +
                          "<color=#ffff00><size=30>宝石是通用货币</size></color>";
            TipsHelper.AddTips(UITransform, "CoreInfo", () => TipsMsg);

            EventManager.Register<CoreInfoChangeEvent>(OnCoreInfoChangeEvent);
        }

        private void RefreshCoreInfo()
        {
            HpText_.text = $"生命值:{(int)PlayerManager.Hp}/{(int)PlayerManager.MaxHp}(+{PlayerManager.HpAdd}/s)";
            MpText_.text = $"魔法值:{(int)PlayerManager.Mp}/{(int)PlayerManager.MaxMp}(+{PlayerManager.MpAdd}/s)";
            GemText_.text = $"金币:{PlayerManager.Gem}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(CoreInfoTrans_);
        }

        private void OnCoreInfoChangeEvent(CoreInfoChangeEvent Event)
        {
            RefreshCoreInfo();
        }



        private void InitWaveInfo()
        {
            WaveInfoTrans_ = FindComponent<RectTransform>("WaveInfo");
            NewWaveText_ = FindComponent<Text>("NewWave");
            CurrentWaveText_ = FindComponent<Text>("WaveInfo/CurrentWave");
            RemainingNumText_ = FindComponent<Text>("WaveInfo/RemainingNum");
            RewardText_ = FindComponent<Text>("WaveInfo/Reward");

            NewWaveText_.gameObject.SetActive(false);

            EventManager.Register<WaveChangeEvent>(OnWaveChangeEvent);
            EventManager.Register<NewWaveEvent>(OnNewWaveEvent);
        }

        private void RefreshWaveInfo()
        {
            CurrentWaveText_.text = $"当前波数：{WaveManager.GetWave().Wave}";
            RemainingNumText_.text = $"剩余数量：{WaveManager.GetWave().GetRemainingCount()}";
            RewardText_.text = "奖励宝石：1/个";

            LayoutRebuilder.ForceRebuildLayoutImmediate(WaveInfoTrans_);
        }

        private void OnWaveChangeEvent(WaveChangeEvent Event)
        {
            RefreshWaveInfo();
        }

        private void OnNewWaveEvent(NewWaveEvent Event)
        {
            NewWaveText_.gameObject.SetActive(true);
            NewWaveText_.text = $"第{WaveManager.GetWave().Wave}波";

            NewWaveText_.transform.ExecuteMotion(new SequenceMotion(
                new FadeInMotion(0.8f),
                new WaitTimeMotion(2.0f),
                new FadeOutMotion(0.8f),
                new CallbackMotion(() =>
                {
                    NewWaveText_.gameObject.SetActive(false);
                })));
        }
    }
}