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
        public BaseExecutor Executor { get; }
        public BaseSelector Selector { get; }

        public SkillDescriptor(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, BaseExecutor Executor, BaseSelector Selector)
        {
            this.SkillID = SkillID;
            this.Name = Name;
            this.Icon = Icon;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
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

            Add(1001, "荆棘", new SkillExecutor_1001());
            Add(1002, "铠甲", new SkillExecutor_1002());
            Add(1003, "固守", new SkillExecutor_1003());
            Add(1004, "活力", new SkillExecutor_1004());
            Add(1005, "冥想", new SkillExecutor_1005());
            Add(1006, "清醒", new SkillExecutor_1006());
            Add(1007, "魔术", new SkillExecutor_1007());

            Add(2001, "镭射激光", "skill1", 5, 30, 0, new SkillExecutor_2001(), new ClickSelector());
            Add(2002, "自动弹幕", "skill2", 5, 30, 0, new SkillExecutor_2002(), new ClickSelector());
            Add(2003, "放马过来", "skill3", 5, 30, 0, new SkillExecutor_2003(), new ClickSelector());
            Add(2004, "天降正义", "skill4", 5, 30, 250, new SkillExecutor_2004(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2005, "速速退散", "skill5", 5, 30, 200, new SkillExecutor_2005(), new DragDirectionSelector("prefabs/bv1.prefab", Configure.CoreBasePosition));
            Add(2006, "减速陷阱", "skill6", 5, 30, 100, new SkillExecutor_2006(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2007, "召唤援军", "skill7", 5, 30, 0, new SkillExecutor_2007(), new ClickSelector());
            Add(2008, "持续子弹", "skill8", 0.1f, 2, 0, new SkillExecutor_2008(), new PressedSelector());

            Add(3001, "普攻", 1, 0, 0, new SkillExecutor_3001());
            Add(3002, "嘲讽", 5, 0, 300, new SkillExecutor_3002());
        }

        public static void PatchQuickController(QuickController Controller)
        {
            Controller.AddNode(new QuickIndex(1, 1, 0, 0, 1), new QuickNode(2001));
            Controller.AddNode(new QuickIndex(0, 1, 0, 2, 1), new QuickNode(2002));
            Controller.AddNode(new QuickIndex(0, 1, 0, 0, 0), new QuickNode(2003));
            Controller.AddNode(new QuickIndex(1, 1, 0, 0, 0), new QuickNode(2004));
            Controller.AddNode(new QuickIndex(0, 1, 1, 0, 1), new QuickNode(2005));
            Controller.AddNode(new QuickIndex(0, 0, 2, 0, 1), new QuickNode(2006));
            Controller.AddNode(new QuickIndex(1, 1, 1, 1, 1), new QuickNode(2007));
            Controller.AddNode(new QuickIndex(0, 0, 0, 1, 0), new QuickNode(2008));
        }

        public static SkillDescriptor Get(uint SkillID)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return SkillList_[SkillID];
            }

            return null;
        }

        private static void Add(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, BaseExecutor Executor, BaseSelector Selector)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return;
            }

            SkillList_.Add(SkillID, new SkillDescriptor(SkillID, Name, $"textures/icon/{Icon}.png", CD, Cost, Radius, Executor, Selector));
        }

        private static void Add(uint SkillID, string Name, BaseExecutor Executor)
        {
            Add(SkillID, Name, string.Empty, 0, 0, 0, Executor, null);
        }

        private static void Add(uint SkillID, string Name, float CD, int Cost, float Radius, BaseExecutor Executor)
        {
            Add(SkillID, Name, string.Empty, CD, Cost, Radius, Executor, null);
        }
    }
}