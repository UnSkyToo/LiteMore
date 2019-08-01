using System.Collections.Generic;
using LiteMore.Combat.Skill;
using LiteMore.Core;

namespace LiteMore.Data
{
    public class SkillData : BaseObject
    {
        public string Icon { get; }
        public string Name { get; }
        public float CD { get; }
        public float Cost { get; }
        public uint Level { get; }
        public QuickIndex Index { get; }

        public SkillData(string Icon, QuickIndex Index)
            : base()
        {
            this.Icon = Icon;
            this.Index = Index;
        }
    }

    public static class SkillLibrary
    {
        private static readonly Dictionary<uint, SkillData> SkillList_ = new Dictionary<uint, SkillData>();

        public static void PatchQuickController(QuickController Controller)
        {
            foreach (var Skill in SkillList_)
            {
                Controller.AddNode(Skill.Value.Index, new QuickNode(Skill.Key));
            }
        }

        public static SkillData Get(uint ID)
        {
            return SkillList_[ID];
        }

        public static void Add(SkillData Data)
        {
            SkillList_.Add(Data.ID, Data);
        }

        public static void Generate()
        {
            Add(new SkillData("Textures/Icon/skill1", new QuickIndex(1, 2, 0, 0, 0)));
            Add(new SkillData("Textures/Icon/skill2", new QuickIndex(1, 1, 0, 0, 0)));
            Add(new SkillData("Textures/Icon/skill3", new QuickIndex(1, 0, 0, 0, 0)));
        }
    }
}