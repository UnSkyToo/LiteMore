using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Npc
{
    public class NpcBar
    {
        private readonly BaseNpc Npc_;
        private readonly Slider HpBar_;
        private readonly Slider MpBar_;

        public NpcBar(BaseNpc Npc)
        {
            Npc_ = Npc;
            HpBar_ = Npc_.GetComponent<Slider>("HpBar");
            HpBar_.minValue = 0;
            MpBar_ = Npc_.GetComponent<Slider>("MpBar");
            MpBar_.minValue = 0;
        }

        public void Tick(float DeltaTime)
        {
            HpBar_.value = Npc_.CalcFinalAttr(NpcAttrIndex.Hp);
            MpBar_.value = Npc_.CalcFinalAttr(NpcAttrIndex.Mp);
        }

        public void SetMaxHp(float MaxHp)
        {
            HpBar_.maxValue = Mathf.Clamp(MaxHp, 1, MaxHp);
        }

        public void SetMaxMp(float MaxMp)
        {
            MpBar_.maxValue = Mathf.Clamp(MaxMp, 1, MaxMp);
            if (Mathf.Approximately(MaxMp, 0))
            {
                MpBar_.gameObject.SetActive(false);
            }
            else
            {
                MpBar_.gameObject.SetActive(true);
            }
        }
    }
}