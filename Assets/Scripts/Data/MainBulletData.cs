using System.Collections.Generic;
using UnityEngine;

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
            15,
            20,
            25,
            30,
            35,
            40,
            45,
            50,
            55,
            80,
            90,
            100,
            110,
            120,
            130,
            140,
            150,
            160,
            170,
            200,
            220,
            240,
            260,
            280,
            300,
            320,
            340,
            360,
            380,
            450,
            500,
            550,
            600,
            650,
            700,
            750,
            800,
            850,
            900,
            1000,
            1100,
            1200,
            1300,
            1400,
            1500,
            1600,
            1700,
            1800,
            1900,
            2000,
            2200,
            2400,
            2600,
            2800,
            3000,
            3200,
            3400,
            3600,
            3800,
            4000,
            4300,
            4600,
            4900,
            5200,
            5500,
            5800,
            6100,
            6400,
            6700,
            7000,
            7400,
            7800,
            8200,
            8600,
            9000,
            9400,
            9800,
            10200,
            10600,
            11500,
            12000,
            12500,
            13000,
            13500,
            14000,
            14500,
            15000,
            15500,
            16000,
            18000,
            19000,
            20000,
            21000,
            22000,
            23000,
            24000,
            25000,
            26000,
            0,
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
            return 5.0f + (Level - 1) * 1.0f;
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
            50,
            100,
            150,
            250,
            350,
            500,
            700,
            950,
            1250,
            1600,
            2000,
            2500,
            4000,
            5000,
            7000,
            10000,
            14000,
            19000,
            23000,
            30000,
            38000,
            45000,
            55000,
            66000,
            0,
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
            return 1.5f - (Level - 1) * 0.05f;
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
            200,
            2000,
            20000,
            200000,
            0,
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

    public class MainBulletData
    {
        public Dictionary<uint, MainBulletDamageData> Damage { get; }
        public Dictionary<uint, MainBulletIntervalData> Interval { get; }
        public Dictionary<uint, MainBulletCountData> Count { get; }

        public MainBulletData(uint MaxWaveLv)
        {
            Damage = new Dictionary<uint, MainBulletDamageData>();
            var MaxBulletDamageLv = (int)Mathf.Pow(MaxWaveLv, 0.8705f);
            for (var Lv = 1u; Lv <= MaxBulletDamageLv; ++Lv)
            {
                Damage.Add(Lv, new MainBulletDamageData(Lv));
            }

            Interval = new Dictionary<uint, MainBulletIntervalData>();
            var MaxBulletIntervalLv = (int)Mathf.Pow(MaxWaveLv, 0.6145f);
            for (var Lv = 1u; Lv <= MaxBulletIntervalLv; ++Lv)
            {
                Interval.Add(Lv, new MainBulletIntervalData(Lv));
            }

            Count = new Dictionary<uint, MainBulletCountData>();
            var MaxBulletCountLv = (int)Mathf.Pow(MaxWaveLv, 0.3115f);
            for (var Lv = 1u; Lv <= MaxBulletCountLv; ++Lv)
            {
                Count.Add(Lv, new MainBulletCountData(Lv));
            }
        }
    }
}