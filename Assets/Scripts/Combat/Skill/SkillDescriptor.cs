namespace LiteMore.Combat.Skill
{
    public struct SkillDescriptor
    {
        public string Name { get; }
        public float CD { get; }
        public int Cost { get; }
        public int Radius { get; }
        public string About { get; }

        public SkillDescriptor(string Name, float CD, int Cost)
            : this(Name, CD, Cost, 0, string.Empty)
        {
        }

        public SkillDescriptor(string Name, float CD, int Cost, int Radius)
            : this(Name, CD, Cost, Radius, string.Empty)
        {
        }

        public SkillDescriptor(string Name, float CD, int Cost, int Radius, string About)
        {
            this.Name = Name;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.About = About;
        }
    }
}