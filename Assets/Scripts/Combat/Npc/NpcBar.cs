using LiteFramework.Game.Asset;
using LiteFramework.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Npc
{
    public class NpcBar : ITick, System.IDisposable
    {
        private readonly BaseNpc Master_;
        private readonly Transform Root_;
        private readonly GameObject HpBarObj_;
        private readonly Slider HpBar_;
        private readonly GameObject MpBarObj_;
        private readonly Slider MpBar_;

        public NpcBar(BaseNpc Master, Vector2 Position, float Scale = 1.0f)
        {
            Master_ = Master;

            Root_ = new GameObject("Bar").transform;
            Root_.transform.SetParent(Master.FrontLayer, false);
            Root_.transform.localPosition = Position;
            Root_.transform.localScale = new Vector3(Scale, Scale, Scale);
            
            HpBarObj_ = AssetManager.CreatePrefabSync(new AssetUri("prefabs/hpbar.prefab"));
            HpBarObj_.transform.SetParent(Root_, false);
            MpBarObj_ = AssetManager.CreatePrefabSync(new AssetUri("prefabs/mpbar.prefab"));
            MpBarObj_.transform.SetParent(Root_, false);
            MpBarObj_.transform.localPosition = new Vector3(0, -12, 0);

            HpBar_ = HpBarObj_.GetComponent<Slider>();
            HpBar_.minValue = 0;
            MpBar_ = MpBarObj_.GetComponent<Slider>();
            MpBar_.minValue = 0;
        }

        public void Tick(float DeltaTime)
        {
            HpBar_.value = Master_.CalcFinalAttr(NpcAttrIndex.Hp);
            MpBar_.value = Master_.CalcFinalAttr(NpcAttrIndex.Mp);
        }

        public void Dispose()
        {
            AssetManager.DeleteAsset(HpBarObj_);
            AssetManager.DeleteAsset(MpBarObj_);
            Object.Destroy(Root_);
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

        public void SetScale(float Scale)
        {
            Root_.transform.localScale = new Vector3(Scale, Scale, Scale);
        }
    }
}