using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Npc
{
    public class NpcBar
    {
        private readonly BaseNpc Master_;
        private readonly Slider HpBar_;
        private readonly Slider MpBar_;

        public NpcBar(BaseNpc Master)
        {
            Master_ = Master;
            HpBar_ = Master_.GetComponent<Slider>("HpBar");
            HpBar_.minValue = 0;
            MpBar_ = Master_.GetComponent<Slider>("MpBar");
            MpBar_.minValue = 0;
        }

        public void Tick(float DeltaTime)
        {
            HpBar_.value = Master_.CalcFinalAttr(NpcAttrIndex.Hp);
            MpBar_.value = Master_.CalcFinalAttr(NpcAttrIndex.Mp);
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