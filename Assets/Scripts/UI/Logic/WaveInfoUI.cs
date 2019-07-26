using LiteMore.Combat.Wave;
using LiteMore.Helper;
using LiteMore.Motion;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class WaveInfoUI : BaseUI
    {
        private Text NewWaveText_;
        private Text WaveText_;
        private Text NumText_;
        private Text GemText_;

        public WaveInfoUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;
        }

        protected override void OnOpen(params object[] Params)
        {
            NewWaveText_ = FindComponent<Text>("NewWave");
            WaveText_ = FindComponent<Text>("BG/Wave");
            NumText_ = FindComponent<Text>("BG/Num");
            GemText_ = FindComponent<Text>("BG/Gem");

            NewWaveText_.gameObject.SetActive(false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(UIRectTransform);

            var TipsMsg = $"<color=#ff0000><size=30>Wave:当前波数</size></color>\n" +
                          "<color=#00ff00><size=30>Num:剩余出怪数量</size></color>\n" +
                          "<color=#ffff00><size=30>Gem:死亡掉落宝石数量</size></color>";

            UIHelper.AddTips(UITransform, "BG", () => TipsMsg);

            EventManager.Register<WaveChangeEvent>(OnWaveChangeEvent);
            EventManager.Register<NewWaveEvent>(OnNewWaveEvent);
        }

        protected override void OnClose()
        {
            EventManager.UnRegister<WaveChangeEvent>(OnWaveChangeEvent);
            EventManager.UnRegister<NewWaveEvent>(OnNewWaveEvent);
        }

        private void OnWaveChangeEvent(WaveChangeEvent Event)
        {
            WaveText_.text = $"{"Wave".Localize()}:{WaveManager.GetWave().Wave}";
            NumText_.text = $"{"RemainNum".Localize()}:{WaveManager.GetWave().GetRemainingCount()}";
            GemText_.text = $"{"Gem".Localize()}:1";
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