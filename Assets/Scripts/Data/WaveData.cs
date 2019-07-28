namespace LiteMore.Data
{
    public class WaveData
    {
        public uint Wave { get; }
        public int TriggerCount { get; }
        public uint EmiiterCount { get; }
        public float Interval { get; }
        public float Speed { get; }
        public int Hp { get; }
        public int Damage { get; }
        public int Gem { get; }

        public WaveData(uint Wave)
        {
            this.Wave = Wave;
            this.TriggerCount = Formula_TriggerCount(Wave);
            this.EmiiterCount = Formula_EmiiterCount(Wave);
            this.Interval = Formula_Interval(Wave);
            this.Speed = Formula_Speed(Wave);
            this.Hp = Formula_Hp(Wave);
            this.Damage = Formula_Damage(Wave);
            this.Gem = Formula_Gem(Wave);
        }

        public static int Formula_TriggerCount(uint Wave)
        {
            return 4 + (int)(Wave / 10);
        }

        public static uint Formula_EmiiterCount(uint Wave)
        {
            return 5 + Wave / 3;
        }

        public static float Formula_Interval(uint Wave)
        {
            return 30.0f / Formula_TriggerCount(Wave);
        }

        public static float Formula_Speed(uint Wave)
        {
            return 70;
        }

        public static int Formula_Hp(uint Wave)
        {
            return 5 + (int)(Wave / 2) * 1 + (int)(Wave / 10) * 5 + (int)(Wave / 20) * 10 + (int)(Wave / 50) * 30;
        }

        public static int Formula_Damage(uint Wave)
        {
            return 1 + (int)(Wave / 5);
        }

        public static int Formula_Gem(uint Wave)
        {
            return 1 + (int)(Wave / 50);
        }
    }
}