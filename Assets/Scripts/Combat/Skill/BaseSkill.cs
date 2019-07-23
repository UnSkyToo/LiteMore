using LiteMore.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Skill
{
    public abstract class BaseSkill
    {
        public string Tips { get; set; }

        protected readonly Transform Transform_;
        protected readonly RectTransform IconTransform_;
        private readonly Image Mask_;
        private readonly GameObject BG_;
        private readonly Text CDText_;

        private float Time_;
        protected bool IsCD_;
        protected readonly SkillDescriptor Desc_;

        protected BaseSkill(Transform Trans, SkillDescriptor Desc)
        {
            Transform_ = Trans;
            IconTransform_ = Trans.GetComponent<RectTransform>();
            Time_ = Desc_.CD;
            IsCD_ = false;
            Tips = TipsHelper.Skill(Desc);

            Desc_ = Desc;

            BG_ = Transform_.Find("BG").gameObject;
            BG_.SetActive(false);
            Mask_ = Transform_.Find("Mask").GetComponent<Image>();
            CDText_ = Transform_.Find("CD").GetComponent<Text>();

            ClearCD();

            UIHelper.AddTips(Transform_, () => Tips);
        }

        public virtual void Destroy()
        {
            Object.Destroy(Transform_.gameObject);
        }

        public virtual void Tick(float DeltaTime)
        {
            if (PlayerManager.Mp < Desc_.Cost)
            {
                BG_.SetActive(true);
            }
            else
            {
                BG_.SetActive(false);
            }

            if (!IsCD_)
            {
                return;
            }

            Time_ -= DeltaTime;
            if (Time_ <= 0.0f)
            {
                Time_ = 0;
                ClearCD();
            }

            Mask_.fillAmount = (1 - Time_ / Desc_.CD);
            CDText_.text = $"{Time_:0.0}s";
        }

        public void StartCD()
        {
            IsCD_ = true;
            Time_ = Desc_.CD;
            CDText_.gameObject.SetActive(true);
            CDText_.text = $"{Time_:0.0}s";
            Mask_.fillAmount = 0;
        }

        public void ClearCD()
        {
            IsCD_ = false;
            CDText_.gameObject.SetActive(false);
            Mask_.fillAmount = 1;
        }
    }
}