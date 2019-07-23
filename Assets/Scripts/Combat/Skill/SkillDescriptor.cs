namespace LiteMore.Combat.Skill
{
    public struct SkillDescriptor
    {
        public string Name { get; }
        public float CD { get; }
        public int Cost { get; }
        public int Radius { get; }

        public SkillDescriptor(string Name, float CD, int Cost)
            : this(Name, CD, Cost, 0)
        {
        }

        public SkillDescriptor(string Name, float CD, int Cost, int Radius)
        {
            this.Name = Name;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
        }
    }
}