using System.Collections.Generic;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Shape;
using LiteMore.Combat.Skill.Executor;
using LiteMore.Combat.Skill.Selector;

namespace LiteMore.Combat.Skill
{
    public class SkillDescriptor
    {
        public uint SkillID { get; }
        public SkillType Type { get; }
        public string Name { get; }
        public string Icon { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }
        public uint Priority { get; }
        public BaseShape Shape { get; }
        public FilterRule Rule { get; }
        public BaseExecutor Executor { get; }
        public BaseSelector Selector { get; }

        public SkillDescriptor(uint SkillID, SkillType Type, string Name, string Icon, float CD, int Cost, float Radius, uint Priority, FilterRule Rule, BaseExecutor Executor, BaseSelector Selector)
        {
            this.SkillID = SkillID;
            this.Type = Type;
            this.Name = Name;
            this.Icon = Icon;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.Rule = Rule;
            this.Priority = Priority;
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

            Add(1001, SkillType.Attack, "普攻", "attack1", 1, 0, 0, 0, new FilterRule(FilterTeamType.Enemy, FilterRangeType.InDistance, FilterNpcType.Nearest), new SkillExecutor_1001(), new ClickSelector());
            

            Add(2001, SkillType.Normal, "镭射激光", "skill1", 5, 30, 0, 1, FilterRule.All, new SkillExecutor_2001(), new ClickSelector());
            Add(2002, SkillType.Normal, "自动弹幕", "skill2", 5, 30, 0, 1, FilterRule.All, new SkillExecutor_2002(), new ClickSelector());
            Add(2003, SkillType.Normal, "放马过来", "skill3", 5, 30, 0, 1, FilterRule.All, new SkillExecutor_2003(), new ClickSelector());
            Add(2004, SkillType.Normal, "天降正义", "skill4", 5, 30, 250, 1, FilterRule.All, new SkillExecutor_2004(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2005, SkillType.Normal, "速速退散", "skill5", 5, 30, 200, 1, FilterRule.All, new SkillExecutor_2005(), new DragDirectionSelector("prefabs/bv1.prefab"));
            Add(2006, SkillType.Normal, "减速陷阱", "skill6", 5, 30, 100, 1, FilterRule.All, new SkillExecutor_2006(), new DragPositionSelector("prefabs/bv0.prefab"));
            Add(2007, SkillType.Normal, "召唤援军", "skill7", 30, 30, 0, 1, FilterRule.All, new SkillExecutor_2007(), new ClickSelector());
            Add(2008, SkillType.Normal, "持续子弹", "skill8", 0.1f, 2, 0, 1, FilterRule.Nearest, new SkillExecutor_2008(), new PressedSelector());
            Add(2009, SkillType.Normal, "嘲讽", "skill9", 5, 30, 300, 1, new FilterRule(FilterTeamType.Enemy, FilterRangeType.InDistance, FilterNpcType.All), new SkillExecutor_2009(), new ClickSelector());
            Add(2010, SkillType.Normal, "影分身", "skill10", 10, 30, 0, 2, FilterRule.All, new SkillExecutor_2010(), new ClickSelector());


            AddPassive(3001, "荆棘", FilterRule.Self, new SkillExecutor_3001());
            AddPassive(3002, "铠甲", FilterRule.Self, new SkillExecutor_3002());
            AddPassive(3003, "固守", FilterRule.Self, new SkillExecutor_3003());
            AddPassive(3004, "活力", FilterRule.Self, new SkillExecutor_3004());
            AddPassive(3005, "冥想", FilterRule.Self, new SkillExecutor_3005());
            AddPassive(3006, "清醒", FilterRule.Self, new SkillExecutor_3006());
            AddPassive(3007, "魔术", FilterRule.Self, new SkillExecutor_3007());
            AddPassive(3008, "大师", FilterRule.Self, new SkillExecutor_3008());
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

        private static void Add(uint SkillID, SkillType Type, string Name, string Icon, float CD, int Cost, float Radius, uint Priority, FilterRule Rule, BaseExecutor Executor, BaseSelector Selector)
        {
            if (SkillList_.ContainsKey(SkillID))
            {
                return;
            }

            SkillList_.Add(SkillID, new SkillDescriptor(SkillID, Type, Name, $"textures/icon/{Icon}.png", CD, Cost, Radius, Priority, Rule, Executor, Selector));
        }

        private static void AddPassive(uint SkillID, string Name, FilterRule Rule, BaseExecutor Executor)
        {
            Add(SkillID, SkillType.Passive, Name, string.Empty, 0, 0, float.MaxValue, 0, Rule, Executor, null);
        }
    }
}