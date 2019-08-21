using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteFramework.Game.UI;
using LiteMore.Combat;
using LiteMore.Combat.Skill;
using LiteMore.Player;
using LiteMore.UI.Item;
using UnityEngine;

namespace LiteMore.UI.Logic
{
    public class MainOperatorUI : BaseUI
    {
        private Transform SkillListObj_;
        private RectTransform CancelObj_;
        private List<SkillIconItem> SkillIconList_;

        public MainOperatorUI()
            : base()
        {
        }

        protected override void OnOpen(params object[] Params)
        {
            SkillListObj_ = FindChild("SkillList");
            CancelObj_ = FindComponent<RectTransform>("Cancel");
            CancelObj_.gameObject.SetActive(false);
            SkillIconList_ = new List<SkillIconItem>();
            EventManager.Register<NpcSkillChangedEvent>(OnNpcSkillChangedEvent);

            Refresh();
        }

        protected override void OnClose()
        {
            ClearAllSkillIcon();
            EventManager.UnRegister<NpcSkillChangedEvent>(OnNpcSkillChangedEvent);
        }

        protected override void OnTick(float DeltaTime)
        {
            foreach (var Icon in SkillIconList_)
            {
                Icon.Tick(DeltaTime);
            }
        }

        private void Refresh()
        {
            ClearAllSkillIcon();

            var SkillList = PlayerManager.Master.Skill.GetSkillList();
            foreach (var Skill in SkillList)
            {
                if (Skill.Type != SkillType.Passive)
                {
                    AddSkillIcon(Skill);
                }
            }
        }

        private void ClearAllSkillIcon()
        {
            foreach (var Icon in SkillIconList_)
            {
                Icon.Dispose();
            }
            SkillIconList_.Clear();
        }

        private void AddSkillIcon(BaseSkill Skill)
        {
            if (Skill is NpcSkill)
            {
                var Icon = new SkillIconItem(SkillListObj_, Skill, CancelObj_, true);
                SkillIconList_.Add(Icon);
            }
        }

        private void OnNpcSkillChangedEvent(NpcSkillChangedEvent Event)
        {
            if (Event.Master.ID != PlayerManager.Master.ID)
            {
                return;
            }

            Refresh();
        }
    }
}