using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Combat.Skill
{
    public class ClickSkill : BaseSkill
    {
        public event System.Action<SkillDescriptor> OnUsed;

        public ClickSkill(Transform Trans, SkillDescriptor Desc)
            : base(Trans, Desc)
        {
            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.Click, (Obj, Pos) => { Use(); });
        }

        public override void Destroy()
        {
            UIEventTriggerListener.Remove(Transform_);
            base.Destroy();
        }

        private void Use()
        {
            if (IsCD_)
            {
                return;
            }

            if (PlayerManager.Mp < Desc_.Cost)
            {
                return;
            }

            StartCD();
            PlayerManager.AddMp(-Desc_.Cost);
            OnUsed?.Invoke(Desc_);
        }
    }
}