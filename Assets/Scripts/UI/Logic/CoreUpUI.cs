using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class CoreUpUI : BaseUI
    {
        private Text BulletDamageText_;

        private bool IsAddMainBulletDamage_ = false;

        public CoreUpUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 100;
        }

        protected override void OnOpen(params object[] Params)
        {
            BulletDamageText_ = FindComponent<Text>("BulletDamage");

            AddEvent((Obj, Pos) =>
            {
                UIManager.CloseUI(this);
            });

            AddEventToChild("BtnOk", () =>
            {
                UIManager.CloseUI(this);
            });

            IsAddMainBulletDamage_ = false;
            AddEventToChild("BulletDamage/BtnAdd", () =>
            {
                if (PlayerManager.Gem < PlayerManager.MainBulletDamage * 10)
                {
                    Debug.LogWarning($"宝石小于{PlayerManager.MainBulletDamage * 10}");
                    return;
                }

                PlayerManager.AddGem(-PlayerManager.MainBulletDamage * 10);
                PlayerManager.AddMainBulletDamage();
                IsAddMainBulletDamage_ = true;
                RefreshUI();
            });

            RefreshUI();
        }

        protected override void OnClose()
        {
            if (IsAddMainBulletDamage_)
            {
                PlayerManager.CreateMainEmitter();
            }
        }

        private void RefreshUI()
        {
            BulletDamageText_.text = $"主炮塔伤害：{PlayerManager.MainBulletDamage}";
        }
    }
}