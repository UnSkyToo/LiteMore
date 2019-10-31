using LiteFramework.Game.Asset;
using LiteFramework.Helper;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Skill.Selector;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Item
{
    public class SkillIconItem
    {
        public Transform IconTransform { get; }
        public BaseSkill Skill { get; }

        protected BaseSelector Selector_;
        private readonly Image Mask_;
        private readonly Text CDText_;
        private readonly Text NameText_;

        public SkillIconItem(Transform Parent, BaseSkill Skill, RectTransform CancelObj, bool IsRect/*temp*/)
        {
            this.Skill = Skill;

            IconTransform = AssetManager.CreatePrefabSync(new AssetUri(IsRect ? "prefabs/skillicon.prefab" : "prefabs/newskillIcon.prefab")).transform;
            IconTransform.SetParent(Parent, false);
            UIHelper.GetComponent<Image>(IconTransform, "BG/Icon").sprite = AssetManager.CreateAssetSync<Sprite>(new AssetUri(Skill.Icon));
            UIHelper.GetComponent<Image>(IconTransform, "BG/Mask").sprite = AssetManager.CreateAssetSync<Sprite>(new AssetUri(Skill.Icon));

            Mask_ = IconTransform.Find("BG/Mask").GetComponent<Image>();
            Mask_.fillAmount = 1;

            CDText_ = IconTransform.Find("CD").GetComponent<Text>();
            CDText_.gameObject.SetActive(false);

            NameText_ = IconTransform.Find("Name").GetComponent<Text>();
            NameText_.text = Skill.Name;

            Selector_ = SkillLibrary.Get(Skill.SkillID).Selector.Clone();
            var Args = new SkillArgs(Skill) {CancelObj = CancelObj};
            Selector_.BindCarrier(IconTransform, Args, (SArgs) =>
            {
                Skill.Master.Skill.UseSkill(SArgs);
            });
        }

        public void Dispose()
        {
            Selector_?.Dispose();
            AssetManager.DeleteAsset(IconTransform.gameObject);
        }

        public void Tick(float DeltaTime)
        {
            Selector_?.Tick(DeltaTime);

            if (Skill.IsCD)
            {
                Mask_.fillAmount = (1 - Skill.Time / Skill.CD);
                CDText_.gameObject.SetActive(true);
                CDText_.text = $"{Skill.Time:0.0}s";
            }
            else
            {
                CDText_.gameObject.SetActive(false);
                Mask_.fillAmount = 1;
            }
        }

        public void SetScale(float Scale)
        {
            IconTransform.localScale = new Vector3(Scale, Scale, 1);
        }

        public void SetScaleToSize(Vector2 Size)
        {
            var CurSize = IconTransform.GetComponent<RectTransform>().sizeDelta;
            IconTransform.localScale = new Vector3(Size.x / CurSize.x, Size.y / CurSize.y, 1);
        }
    }
}