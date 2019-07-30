namespace LiteMore.Data
{
    public class MainBulletDamageData
    {
        public uint Level { get; }
        public uint Cost { get; }
        public float Damage { get; }

        private static readonly uint[] CostCfg_ =
        {
            0,
            10,
            26,
            43,
            60,
            77,
            93,
            110,
            127,
            143,
            160,
            177,
            193,
            210,
            227,
            244,
            260,
            277,
            294,
            310,
            327,
            344,
            360,
            377,
            394,
            411,
            427,
            444,
            461,
            477,
            494,
            511,
            527,
            544,
            561,
        };

        public MainBulletDamageData(uint Level)
        {
            this.Level = Level;
            this.Cost = Formula_Cost(Level);
            this.Damage = Formula_Damage(Level);
        }

        public static uint Formula_Cost(uint Level)
        {
            return CostCfg_[Level];
        }

        public static float Formula_Damage(uint Level)
        {
            return 1 + (Level - 1) * 0.5f;
        }
    }

    public class MainBulletIntervalData
    {
        public uint Level { get; }
        public uint Cost { get; }
        public float Interval { get; }

        private static readonly uint[] CostCfg_ =
        {
            0,
            145,
            308,
            470,
            632,
            795,
            957,
            1119,
            1281,
            1444,
            1606,
            1768,
            1930,
            2093,
        };

        public MainBulletIntervalData(uint Level)
        {
            this.Level = Level;
            this.Cost = Formula_Cost(Level);
            this.Interval = Formula_Interval(Level);
        }

        public static uint Formula_Cost(uint Level)
        {
            return CostCfg_[Level];
        }

        public static float Formula_Interval(uint Level)
        {
            return 1.5f - (Level - 1) * 0.025f;
        }
    }

    public class MainBulletCountData
    {
        public uint Level { get; }
        public uint Cost { get; }
        public uint Count { get; }

        private static readonly uint[] CostCfg_ =
        {
            0,
            242,
            4122,
            8002,
            11881,
        };

        public MainBulletCountData(uint Level)
        {
            this.Level = Level;
            this.Cost = Formula_Cost(Level);
            this.Count = Formula_Count(Level);
        }

        public static uint Formula_Cost(uint Level)
        {
            return CostCfg_[Level];
        }

        public static uint Formula_Count(uint Level)
        {
            return Level;
        }
    }
}