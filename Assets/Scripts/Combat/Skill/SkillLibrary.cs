using System.Collections.Generic;
using LiteMore.Combat.Skill.Executor;

namespace LiteMore.Combat.Skill
{
    public class SkillDescriptor
    {
        public uint SkillID { get; }
        public string Name { get; }
        public string Icon { get; }
        public QuickIndex Index { get; }
        public BaseExecutor Executor { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }

        public SkillDescriptor(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, QuickIndex Index, BaseExecutor Executor)
        {
            this.SkillID = SkillID;
            this.Name = Name;
            this.Icon = Icon;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.Index = Index;
            this.Executor = Executor;
        }
    }

    public static class SkillLibrary
    {
        private static readonly Dictionary<uint, SkillDescriptor> SkillList_ = new Dictionary<uint, SkillDescriptor>();

        public static void Generate()
        {
            SkillList_.Clear();

            Add(1001, "镭射激光", "skill1", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1001());
            Add(1002, "自动弹幕", "skill2", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1002());
            Add(1003, "放马过来", "skill3", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1003());
            Add(1004, "天降正义", "skill4", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1004());
            Add(1005, "速速退散", "skill5", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1005());
            Add(1006, "减速陷阱", "skill6", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1006());
            Add(1007, "召唤援军", "skill7", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1007());
            Add(1008, "持续子弹", "skill8", 5, 30, 0, new QuickIndex(), new MainSkillExecutor_1008());
        }

        public static void PatchQuickController(QuickController Controller)
        {
            foreach (var Info in SkillList_)
            {
                Controller.AddNode(Info.Value.Index, new QuickNode(Info.Key));
            }
        }

        public static SkillDescriptor Get(uint SkillID)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return SkillList_[SkillID];
            }

            return null;
        }

        private static void Add(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, QuickIndex Index, BaseExecutor Executor)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return;
            }

            SkillList_.Add(SkillID, new SkillDescriptor(SkillID, Name, $"Textures/Icon/{Icon}", CD, Cost, Radius, Index, Executor));
        }
    }
}