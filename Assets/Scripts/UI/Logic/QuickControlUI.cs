using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Combat.Skill;
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

        private MainSkill CurrentSkill_;

        public QuickControlUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 0;

            Controller_ = new QuickController();
            //SkillLibrary.PatchQuickController(Controller_);
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
                    Img.sprite = Resources.Load<Sprite>("Textures/Icon/b1_metal");
                    break;
                case QuickCode.Wood:
                    Img.sprite = Resources.Load<Sprite>("Textures/Icon/b2_wood");
                    break;
                case QuickCode.Water:
                    Img.sprite = Resources.Load<Sprite>("Textures/Icon/b3_water");
                    break;
                case QuickCode.Fire:
                    Img.sprite = Resources.Load<Sprite>("Textures/Icon/b4_fire");
                    break;
                case QuickCode.Earth:
                    Img.sprite = Resources.Load<Sprite>("Textures/Icon/b5_earth");
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
                ProbeIcon_.sprite = Resources.Load<Sprite>(SkillLibrary.Get(Node.ID).Icon);
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
            SkillManager.RemoveMainSkill(CurrentSkill_);
            var Desc = SkillLibrary.Get(Node.ID);
            CurrentSkill_ = SkillManager.AddMainSkill(Desc);
        }
    }
}