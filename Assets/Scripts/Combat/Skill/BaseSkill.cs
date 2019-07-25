namespace LiteMore.Combat.Skill
{
    public struct BaseSkillDescriptor
    {
        public string Icon { get; }
        public string Name { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }
        public string About { get; }

        public BaseSkillDescriptor(string Icon, string Name, float CD, int Cost)
            : this(Icon, Name, CD, Cost, 0, string.Empty)
        {
        }

        public BaseSkillDescriptor(string Icon, string Name, float CD, int Cost, float Radius)
            : this(Icon, Name, CD, Cost, Radius, string.Empty)
        {
        }

        public BaseSkillDescriptor(string Icon, string Name, float CD, int Cost, float Radius, string About)
        {
            this.Icon = Icon;
            this.Name = Name;
            this.CD = CD;
            this.Cost = Cost;
            this.Radius = Radius;
            this.About = About;
        }
    }

    public class BaseSkill
    {
        public string Icon { get; }
        public string Name { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }
        public string About { get; }
        public float Time { get; protected set; }
        public bool IsCD { get; protected set; }

        public BaseSkill(BaseSkillDescriptor Desc)
        {
            this.Icon = Desc.Icon;
            this.Name = Desc.Name;
            this.CD = Desc.CD;
            this.Cost = Desc.Cost;
            this.Radius = Desc.Radius;
            this.About = Desc.About;
            this.Time = 0;
            this.IsCD = false;
        }

        public virtual void Destroy()
        {
        }

        public virtual void Tick(float DeltaTime)
        {
            if (!IsCD)
            {
                return;
            }

            Time -= DeltaTime;
            if (Time <= 0.0f)
            {
                Time = 0;
                ClearCD();
            }
        }

        public virtual void StartCD()
        {
            Time = CD;
            IsCD = true;
        }

        public virtual void ClearCD()
        {
            IsCD = false;
        }

        public bool CanUse()
        {
            return !IsCD && Cost <= PlayerManager.Mp;
        }

        public bool Use()
        {
            if (!CanUse())
            {
                return false;
            }

            StartCD();
            PlayerManager.AddMp(-Cost);
            return true;
        }
    }
}