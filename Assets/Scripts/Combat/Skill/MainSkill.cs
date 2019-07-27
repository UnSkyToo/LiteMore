using LiteMore.Combat.Skill.Selector;
using LiteMore.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Skill
{
    public class MainSkill : BaseSkill
    {
        public string Tips { get; set; }
        public Transform IconTransform { get; }

        protected BaseSelector Selector_;
        private readonly Image Mask_;
        private readonly GameObject BG_;
        private readonly Text CDText_;

        public MainSkill(Transform Trans, BaseSkillDescriptor Desc)
            : base(Desc)
        {
            IconTransform = Trans;

            Tips = TipsHelper.Skill(Desc);
            BG_ = IconTransform.Find("BG").gameObject;
            BG_.SetActive(false);

            Mask_ = IconTransform.Find("Mask").GetComponent<Image>();
            Mask_.fillAmount = 1;

            CDText_ = IconTransform.Find("CD").GetComponent<Text>();
            CDText_.gameObject.SetActive(false);

            TipsHelper.AddTips(IconTransform, () => Tips);
        }

        public override void Dispose()
        {
            Selector_?.Dispose();
            Object.Destroy(IconTransform.gameObject);
            base.Dispose();
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);
            Selector_?.Tick(DeltaTime);

            if (PlayerManager.Mp < Cost)
            {
                BG_.SetActive(true);
            }
            else
            {
                BG_.SetActive(false);
            }

            if (!IsCD)
            {
                return;
            }

            base.Tick(DeltaTime);

            Mask_.fillAmount = (1 - Time / CD);
            CDText_.text = $"{Time:0.0}s";
        }

        public void SetSelector(BaseSelector Selector)
        {
            this.Selector_ = Selector;
        }

        public override void StartCD()
        {
            base.StartCD();

            CDText_.gameObject.SetActive(true);
            CDText_.text = $"{Time:0.0}s";
            Mask_.fillAmount = 0;
        }

        public override void ClearCD()
        {
            base.ClearCD();

            CDText_.gameObject.SetActive(false);
            Mask_.fillAmount = 1;
        }
    }
}