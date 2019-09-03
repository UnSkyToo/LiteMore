using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Player;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class DpsUI : BaseUI
    {
        private GameObject ItemObj_;
        private Transform ContentRoot_;
        private Text DpsText_;

        private const float Interval_ = 0.5f;
        private float Time_;

        public DpsUI()
            : base()
        {
            DepthMode = UIDepthMode.Normal;
            DepthIndex = 100;
        }

        protected override void OnOpen(params object[] Params)
        {
            ItemObj_ = FindChild("Item").gameObject;
            ItemObj_.SetActive(false);

            ContentRoot_ = FindChild("List/Viewport/Content");

            DpsText_ = FindComponent<Text>("Dps");

            AddEventToChild("BtnClear", () =>
            {
                PlayerManager.Dps.Clear();
                UIHelper.RemoveAllChildren(ContentRoot_);
            });
        }

        protected override void OnTick(float DeltaTime)
        {
            Time_ += DeltaTime;

            if (Time_ >= Interval_)
            {
                Time_ -= Interval_;
                Refresh();
            }
        }

        private void Refresh()
        {
            var Chunks = PlayerManager.Dps.GetChunks();

            for (var Index = 0; Index < ContentRoot_.childCount; ++Index)
            {
                ContentRoot_.GetChild(Index).gameObject.SetActive(false);
            }

            for (var Index = 0; Index < Chunks.Count; ++Index)
            {
                var Child = Index >= ContentRoot_.childCount ? CreateItem() : ContentRoot_.GetChild(Index);

                Child.gameObject.SetActive(true);
                UIHelper.FindComponent<Text>(Child, "Name").text = Chunks[Index].SourceName;
                UIHelper.FindComponent<Text>(Child, "Text").text = $"{Chunks[Index].Value:0.0}({Chunks[Index].Percent*100:0.00}%)";
                UIHelper.FindComponent<Slider>(Child, "Value").value = Chunks[Index].Percent;
            }

            DpsText_.text = $"Dps:{PlayerManager.Dps.Dps}";
        }

        private Transform CreateItem()
        {
            var Obj = Object.Instantiate(ItemObj_);
            Obj.transform.SetParent(ContentRoot_, false);
            UIHelper.FindComponent<Image>(Obj.transform, "Value/Fill Area/Fill").color = new Color(Random.value, 0.8f, Random.value);
            return Obj.transform;
        }
    }
}