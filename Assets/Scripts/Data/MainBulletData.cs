namespace LiteMore.Data
{
    public class MainBulletDamageData
    {
        public uint Level { get; }
        public int Cost { get; }
        public int Damage { get; }

        public MainBulletDamageData(uint Level)
        {
            this.Level = Level;
            this.Cost = Formula_Cost(Level);
            this.Damage = Formula_Damage(Level);
        }

        public static int Formula_Cost(uint Level)
        {
            return 10 + (int)(Level * 10);
        }

        public static int Formula_Damage(uint Level)
        {
            return (int) Level;
        }
    }

    public class MainBulletIntervalData
    {
        public uint Level { get; }
        public int Cost { get; }
        public float Interval { get; }

        public MainBulletIntervalData(uint Level)
        {
            this.Level = Level;
            this.Cost = Formula_Cost(Level);
            this.Interval = Formula_Interval(Level);
        }

        public static int Formula_Cost(uint Level)
        {
            return 10 + (int) (Level * 10);
        }

        public static float Formula_Interval(uint Level)
        {
            return 5.05f - 0.05f * Level;
        }
    }
}