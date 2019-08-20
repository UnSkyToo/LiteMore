using LiteFramework.Game.Asset;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Combat.Npc.Module;
using LiteMore.Combat.Skill;
using LiteMore.Player;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class QuickControlUI : BaseUI
    {
        private readonly QuickController Controller_;
        private GameObject ProbeObj_;
        private Image ProbeIcon_;
        private Text ProbeName_;
        private Transform InputList_;
        private GameObject InputObj_;

        private BaseSkill CurrentSkill_;

        public QuickControlUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;

            Controller_ = new QuickController();
            SkillLibrary.PatchQuickController(Controller_);
        }

        protected override void OnOpen(params object[] Params)
        {
            AddEventToChild("Code/Metal", () => { Controller_.ExecuteCode(QuickCode.Metal); });
            AddEventToChild("Code/Wood", () => { Controller_.ExecuteCode(QuickCode.Wood); });
            AddEventToChild("Code/Water", () => { Controller_.ExecuteCode(QuickCode.Water); });
            AddEventToChild("Code/Fire", () => { Controller_.ExecuteCode(QuickCode.Fire); });
            AddEventToChild("Code/Earth", () => { Controller_.ExecuteCode(QuickCode.Earth); });

            ProbeObj_ = FindChild("Probe").gameObject;
            ProbeObj_.SetActive(false);
            ProbeIcon_ = FindComponent<Image>("Probe/Icon");
            ProbeName_ = FindComponent<Text>("Probe/Name");

            InputList_ = FindChild("Input");
            InputObj_ = FindChild("InputItem").gameObject;
            InputObj_.SetActive(false);

            Controller_.OnFailed += () => { ResetQuickState(); };
            Controller_.OnSucceed += OnQuickSucceed;
            Controller_.OnProbe += (Node) => { UpdateProbe(Node); };
            Controller_.OnCode += (Code) => { AddItemToInput(Code); };
        }

        protected override void OnTick(float DeltaTime)
        {
            Controller_.Tick(DeltaTime);
        }

        private void ResetQuickState()
        {
            UIHelper.RemoveAllChildren(InputList_);
            ProbeObj_.SetActive(false);
        }

        private void AddItemToInput(QuickCode Code)
        {
            var Obj = Object.Instantiate(InputObj_);
            Obj.transform.SetParent(InputList_, false);
            Obj.SetActive(true);
            var Img = Obj.GetComponent<Image>();

            switch (Code)
            {
                case QuickCode.Metal:
                    Img.sprite = AssetManager.CreateAssetSync<Sprite>("textures/icon/b1_metal.png");
                    break;
                case QuickCode.Wood:
                    Img.sprite = AssetManager.CreateAssetSync<Sprite>("textures/icon/b2_wood.png");
                    break;
                case QuickCode.Water:
                    Img.sprite = AssetManager.CreateAssetSync<Sprite>("textures/icon/b3_water.png");
                    break;
                case QuickCode.Fire:
                    Img.sprite = AssetManager.CreateAssetSync<Sprite>("textures/icon/b4_fire.png");
                    break;
                case QuickCode.Earth:
                    Img.sprite = AssetManager.CreateAssetSync<Sprite>("textures/icon/b5_earth.png");
                    break;
                default:
                    break;
            }
        }

        private void UpdateProbe(QuickNode Node)
        {
            if (Node != null)
            {
                ProbeObj_.SetActive(true);
                ProbeIcon_.sprite = AssetManager.CreateAssetSync<Sprite>(SkillLibrary.Get(Node.ID).Icon);
                ProbeName_.text = SkillLibrary.Get(Node.ID).Name;
            }
            else
            {
                ProbeObj_.SetActive(false);
            }
        }

        private void OnQuickSucceed(QuickNode Node)
        {
            ResetQuickState();
            PlayerManager.Master.Skill.RemoveSkill(CurrentSkill_.SkillID, true);
            CurrentSkill_ = PlayerManager.Master.Skill.AddNpcSkill(Node.ID);
        }
    }
}