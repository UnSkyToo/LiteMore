using System.Collections.Generic;
using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Shape;
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
        public BaseShape Shape { get; }
        public LockingRule Rule { get; }
        public BaseExecutor Executor { get; }
        public BaseSelector Selector { get; }

        public SkillDescriptor(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, LockingRule Rule, BaseExecutor Executor, BaseSelector Selector)
        {
            this.SkillID = SkillID;
            this.Name = Name;
            this.Icon = Icon;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.Rule = Rule;
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

            AddPassive(1001, "荆棘", LockingRule.Self, new SkillExecutor_1001());
            AddPassive(1002, "铠甲", LockingRule.Self, new SkillExecutor_1002());
            AddPassive(1003, "固守", LockingRule.Self, new SkillExecutor_1003());
            AddPassive(1004, "活力", LockingRule.Self, new SkillExecutor_1004());
            AddPassive(1005, "冥想", LockingRule.Self, new SkillExecutor_1005());
            AddPassive(1006, "清醒", LockingRule.Self, new SkillExecutor_1006());
            AddPassive(1007, "魔术", LockingRule.Self, new SkillExecutor_1007());

            Add(2001, "镭射激光", "skill1", 5, 30, 0, LockingRule.All, new SkillExecutor_2001(), new ClickSelector());
            Add(2002, "自动弹幕", "skill2", 5, 30, 0, LockingRule.All, new SkillExecutor_2002(), new ClickSelector());
            Add(2003, "放马过来", "skill3", 5, 30, 0, LockingRule.All, new SkillExecutor_2003(), new ClickSelector());
            Add(2004, "天降正义", "skill4", 5, 30, 250, LockingRule.All, new SkillExecutor_2004(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2005, "速速退散", "skill5", 5, 30, 200, LockingRule.All, new SkillExecutor_2005(), new DragDirectionSelector("prefabs/bv1.prefab"));
            Add(2006, "减速陷阱", "skill6", 5, 30, 100, LockingRule.All, new SkillExecutor_2006(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2007, "召唤援军", "skill7", 5, 30, 0, LockingRule.All, new SkillExecutor_2007(), new ClickSelector());
            Add(2008, "持续子弹", "skill8", 0.1f, 2, 0, LockingRule.Nearest, new SkillExecutor_2008(), new PressedSelector());

            Add(3001, "普攻", "skill1", 1, 0, 0, new LockingRule(LockTeamType.Enemy, LockRangeType.InDistance, LockNpcType.Nearest), new SkillExecutor_3001(), new ClickSelector());
            Add(3002, "嘲讽", "skill2", 5, 0, 300, new LockingRule(LockTeamType.Enemy, LockRangeType.InDistance, LockNpcType.All), new SkillExecutor_3002(), new ClickSelector());
            Add(3003, "影分身", "skill3", 8, 0, 0, LockingRule.All, new SkillExecutor_3003(), new ClickSelector());
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

        private static void Add(uint SkillID, string Name, string Icon, float CD, int Cost, float Radius, LockingRule Rule, BaseExecutor Executor, BaseSelector Selector)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return;
            }

            SkillList_.Add(SkillID, new SkillDescriptor(SkillID, Name, $"textures/icon/{Icon}.png", CD, Cost, Radius, Rule, Executor, Selector));
        }

        private static void AddPassive(uint SkillID, string Name, LockingRule Rule, BaseExecutor Executor)
        {
            Add(SkillID, Name, string.Empty, 0, 0, float.MaxValue, Rule, Executor, null);
        }
    }
}