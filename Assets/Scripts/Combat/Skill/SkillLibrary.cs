using System.Collections.Generic;
using LiteMore.Combat.Skill.Executor;
using LiteMore.Combat.Skill.Selector;

namespace LiteMore.Combat.Skill
{
    public class SkillDescriptor
    {
        public uint SkillID { get; }
        public string Name { get; }
        public string Icon { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }
        public QuickIndex Index { get; }
        public BaseExecutor Executor { get; }
        public BaseSelector Selector { get; }

        public SkillDescriptor(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, QuickIndex Index, BaseExecutor Executor, BaseSelector Selector)
        {
            this.SkillID = SkillID;
            this.Name = Name;
            this.Icon = Icon;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.Index = Index;
            this.Executor = Executor;
            this.Selector = Selector;
        }
    }

    public static class SkillLibrary
    {
        private static readonly Dictionary<uint, SkillDescriptor> SkillList_ = new Dictionary<uint, SkillDescriptor>();

        public static void Generate()
        {
            SkillList_.Clear();

            Add(0001, "荆棘", string.Empty, 0, 0, 0, QuickIndex.Null, new SkillExecutor_0001(), null);

            Add(1001, "镭射激光", "skill1", 5, 30, 0, new QuickIndex(2, 0, 0, 2, 1),
                new SkillExecutor_1001(), new ClickSelector());
            Add(1002, "自动弹幕", "skill2", 5, 30, 0, new QuickIndex(1, 1, 0, 1, 0),
                new SkillExecutor_1002(), new ClickSelector());
            Add(1003, "放马过来", "skill3", 5, 30, 0, new QuickIndex(0, 0, 0, 0, 1),
                new SkillExecutor_1003(), new ClickSelector());
            Add(1004, "天降正义", "skill4", 5, 30, 250, new QuickIndex(0, 0, 0, 3, 1),
                new SkillExecutor_1004(), new DragPositionSelector("Prefabs/bv0"));
            Add(1005, "速速退散", "skill5", 5, 30, 200, new QuickIndex(2, 2, 0, 0, 0),
                new SkillExecutor_1005(), new DragDirectionSelector("Prefabs/bv1", Configure.CoreBasePosition));
            Add(1006, "减速陷阱", "skill6", 5, 30, 100, new QuickIndex(0, 0, 3, 0, 1),
                new SkillExecutor_1006(), new DragPositionSelector("Prefabs/bv0"));
            Add(1007, "召唤援军", "skill7", 5, 30, 0, new QuickIndex(1, 1, 1, 1, 1),
                new SkillExecutor_1007(), new ClickSelector());
            Add(1008, "持续子弹", "skill8", 0.1f, 2, 0, new QuickIndex(0, 0, 0, 1, 0),
                new SkillExecutor_1008(), new PressedSelector());
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

        private static void Add(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, QuickIndex Index, BaseExecutor Executor, BaseSelector Selector)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return;
            }

            SkillList_.Add(SkillID, new SkillDescriptor(SkillID, Name, $"Textures/Icon/{Icon}", CD, Cost, Radius, Index, Executor, Selector));
        }
    }
}