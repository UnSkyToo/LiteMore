using LiteFramework;
using LiteFramework.Core.Event;
using LiteFramework.Core.Motion;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Combat.Wave;
using LiteMore.Helper;
using LiteMore.Player;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class MainUI : BaseUI
    {
        private CoreInfoPart CoreInfo_;
        private WaveInfoPart WaveInfo_;
        private CoreUpPart CoreUp_;

        private int TimeScale_ = 1;

        public MainUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;
        }

        protected override void OnOpen(params object[] Params)
        {
            CoreInfo_ = new CoreInfoPart(FindChild("CoreInfo"));
            WaveInfo_ = new WaveInfoPart(FindChild("WaveInfo"));
            CoreUp_ = new CoreUpPart(FindChild("CoreUpInfo"));

            AddEventToChild("BtnDelete", () =>
            {
                if (LiteManager.IsRestart)
                {
                    return;
                }

                if (PlayerManager.DeleteArchive())
                {
                    ToastHelper.Show("删除存档成功", Color.red, LiteManager.Restart);
                }
                else
                {
                    ToastHelper.Show("删除存档失败", Color.red);
                }
            });

            AddEventToChild("BtnSpeed", () =>
            {
                TimeScale_++;
                if (TimeScale_ > 3)
                {
                    TimeScale_ = 1;
                }

                LiteManager.TimeScale = TimeScale_;
                FindComponent<Text>("BtnSpeed/Text").text = $"x{TimeScale_}";
            });
        }

        protected override void OnClose()
        {
            CoreInfo_.Dispose();
            WaveInfo_.Dispose();
            CoreUp_.Dispose();
        }

        private class CoreInfoPart
        {
            private readonly RectTransform Trans_;
            private readonly Text HpText_;
            private readonly Text MpText_;
            private readonly Text GemText_;

            internal CoreInfoPart(Transform Trans)
            {
                Trans_ = Trans.GetComponent<RectTransform>();

                HpText_ =  UIHelper.FindComponent<Text>(Trans, "Hp");
                MpText_ =  UIHelper.FindComponent<Text>(Trans, "Mp");
                GemText_ = UIHelper.FindComponent<Text>(Trans, "Gem");

                var TipsMsg = $"<color=red><size=30>Hp归零游戏结束</size></color>\n" +
                              "<color=green><size=30>Mp用于释放技能</size></color>\n" +
                              "<color=yellow><size=30>Gem是通用货币</size></color>";
                TipsHelper.AddTips(Trans, () => TipsMsg);

                EventManager.Register<CoreInfoChangeEvent>(OnCoreInfoChangeEvent);

                Refresh();
            }

            internal void Dispose()
            {
                EventManager.UnRegister<CoreInfoChangeEvent>(OnCoreInfoChangeEvent);
            }

            private void Refresh()
            {
                HpText_.text = $"Hp:{(int)PlayerManager.Player.Hp}/{(int)PlayerManager.Player.MaxHp}(+{PlayerManager.Player.AddHp}/s)";
                MpText_.text = $"Mp:{(int)PlayerManager.Player.Mp}/{(int)PlayerManager.Player.MaxMp}(+{PlayerManager.Player.AddMp}/s)";
                GemText_.text = $"Gem:{PlayerManager.Player.Gem}";
                LayoutRebuilder.ForceRebuildLayoutImmediate(Trans_);
            }

            private void OnCoreInfoChangeEvent(CoreInfoChangeEvent Event)
            {
                Refresh();
            }
        }

        private class WaveInfoPart
        {
            private readonly RectTransform Trans_;
            private readonly Text WaveText_;
            private readonly Text RemainingNumText_;
            private readonly Text IntervalText_;
            private readonly Text SpeedText_;
            private readonly Text HpText_;
            private readonly Text DamageText_;
            private readonly Text GemText_;
            private readonly Text NewWaveText_;
            private readonly GameObject BtnNextWaveObj_;

            internal WaveInfoPart(Transform Trans)
            {
                Trans_ = Trans.GetComponent<RectTransform>();

                WaveText_ = UIHelper.FindComponent<Text>(Trans, "List/Wave");
                RemainingNumText_ = UIHelper.FindComponent<Text>(Trans, "List/RemainingNum");
                IntervalText_ = UIHelper.FindComponent<Text>(Trans, "List/Interval");
                SpeedText_ = UIHelper.FindComponent<Text>(Trans, "List/Speed");
                HpText_ = UIHelper.FindComponent<Text>(Trans, "List/Hp");
                DamageText_ = UIHelper.FindComponent<Text>(Trans, "List/Damage");
                GemText_ = UIHelper.FindComponent<Text>(Trans, "List/Gem");
                NewWaveText_ = UIHelper.FindComponent<Text>(Trans, "NewWave");
                NewWaveText_.gameObject.SetActive(false);
                BtnNextWaveObj_ = UIHelper.FindChild(Trans, "BtnNextWave").gameObject;

                UIHelper.AddEvent(BtnNextWaveObj_.transform, OnBtnNextWaveClick);

                EventManager.Register<WaveChangeEvent>(OnWaveChangeEvent);
                EventManager.Register<NewWaveEvent>(OnNewWaveEvent);
            }

            internal void Dispose()
            {
                EventManager.UnRegister<WaveChangeEvent>(OnWaveChangeEvent);
                EventManager.UnRegister<NewWaveEvent>(OnNewWaveEvent);
            }

            private void Refresh()
            {
                var Data = WaveManager.GetWave()?.Data;
                if (Data == null)
                {
                    return;
                }

                WaveText_.text = $"当前波数：<color=red>{Data.Wave}</color>";
                RemainingNumText_.text = $"剩余数量：{WaveManager.GetWave().GetRemainingCount()}";
                IntervalText_.text = $"间隔时间：{Data.Interval:0.0}s";
                SpeedText_.text = $"移动速度：{Data.Speed}";
                HpText_.text = $"生命值：{Data.Hp}";
                DamageText_.text = $"伤害值：{Data.Damage}";
                GemText_.text = $"奖励宝石：{Data.Gem}";

                LayoutRebuilder.ForceRebuildLayoutImmediate(Trans_);
            }

            private void OnWaveChangeEvent(WaveChangeEvent Event)
            {
                Refresh();
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

            private void OnBtnNextWaveClick()
            {
                PlayerManager.AddWave();
            }
        }

        private class CoreUpPart
        {
            private readonly Text BulletDamageText_;
            private readonly Text BulletIntervalText_;
            private readonly Text BulletCountText_;

            internal CoreUpPart(Transform Trans)
            {
                BulletDamageText_ = UIHelper.FindComponent<Text>(Trans, "MainBullet/Damage/Text");
                UIHelper.AddEventToChild(Trans, "MainBullet/Damage/BtnAdd", OnBulletDamageBtnAdd);

                BulletIntervalText_ = UIHelper.FindComponent<Text>(Trans, "MainBullet/Interval/Text");
                UIHelper.AddEventToChild(Trans, "MainBullet/Interval/BtnAdd", OnBulletIntervalBtnAdd);

                BulletCountText_ = UIHelper.FindComponent<Text>(Trans, "MainBullet/Count/Text");
                UIHelper.AddEventToChild(Trans, "MainBullet/Count/BtnAdd", OnBulletCountBtnAdd);

                Refresh();
            }

            internal void Dispose()
            {
            }

            private void Refresh()
            {
                BulletDamageText_.text = $"主炮塔射击伤害(Lv{PlayerManager.GetBulletDamageLevel()})：{PlayerManager.GetBulletDamage()}（<color=yellow>{PlayerManager.GetBulletDamageCost()}</color>）";
                BulletIntervalText_.text = $"主炮塔射击间隔(Lv{PlayerManager.GetBulletIntervalLevel()})：{PlayerManager.GetBulletInterval()}s（<color=yellow>{PlayerManager.GetBulletIntervalCost()}</color>）";
                BulletCountText_.text = $"主炮塔射击数量(Lv{PlayerManager.GetBulletCountLevel()})：{PlayerManager.GetBulletCount()}（<color=yellow>{PlayerManager.GetBulletCountCost()}</color>）";
            }

            private void OnBulletDamageBtnAdd()
            {
                if (PlayerManager.Player.Gem >= PlayerManager.GetBulletDamageCost())
                {
                    PlayerManager.AddGem(-(int)PlayerManager.GetBulletDamageCost());
                    PlayerManager.AddBulletDamageLevel();

                    Refresh();
                }
                else
                {
                    ToastHelper.Show($"宝石不足{PlayerManager.GetBulletDamageCost()}，无法升级", Color.red);
                }
            }

            private void OnBulletIntervalBtnAdd()
            {
                if (PlayerManager.Player.Gem >= PlayerManager.GetBulletIntervalCost())
                {
                    PlayerManager.AddGem(-(int)PlayerManager.GetBulletIntervalCost());
                    PlayerManager.AddBulletIntervalLevel();

                    Refresh();
                }
                else
                {
                    ToastHelper.Show($"宝石不足{PlayerManager.GetBulletIntervalCost()}，无法升级", Color.red);
                }
            }

            private void OnBulletCountBtnAdd()
            {
                if (PlayerManager.Player.Gem >= PlayerManager.GetBulletCountCost())
                {
                    PlayerManager.AddGem(-(int)PlayerManager.GetBulletCountCost());
                    PlayerManager.AddBulletCountLevel();

                    Refresh();
                }
                else
                {
                    ToastHelper.Show($"宝石不足{PlayerManager.GetBulletCountCost()}，无法升级", Color.red);
                }
            }
        }
    }
}