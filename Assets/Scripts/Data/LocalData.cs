using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Data
{
    public static class LocalData
    {
        private const int MaxWave = 100;
        private static readonly int MaxBulletDamageLv = (int)Mathf.Pow(MaxWave, 0.7671f);
        private static readonly int MaxBulletIntervalLv = (int)Mathf.Pow(MaxWave, 0.5664f);
        private static readonly int MaxBulletCountLv = (int)Mathf.Pow(MaxWave, 0.3334f);

        public static Dictionary<uint, WaveData> WaveList = new Dictionary<uint, WaveData>();
        public static Dictionary<uint, MainBulletDamageData> MainBulletDamage = new Dictionary<uint, MainBulletDamageData>();
        public static Dictionary<uint, MainBulletIntervalData> MainBulletInterval = new Dictionary<uint, MainBulletIntervalData>();
        public static Dictionary<uint, MainBulletCountData> MainBulletCount = new Dictionary<uint, MainBulletCountData>();

        public static void Generate()
        {
            WaveList.Clear();
            for (var Wave = 1u; Wave <= MaxWave; ++Wave)
            {
                WaveList.Add(Wave, new WaveData(Wave));
            }

            MainBulletDamage.Clear();
            for (var Lv = 1u; Lv <= MaxBulletDamageLv; ++Lv)
            {
                MainBulletDamage.Add(Lv, new MainBulletDamageData(Lv));
            }

            MainBulletInterval.Clear();
            for (var Lv = 1u; Lv <= MaxBulletIntervalLv; ++Lv)
            {
                MainBulletInterval.Add(Lv, new MainBulletIntervalData(Lv));
            }

            MainBulletCount.Clear();
            for (var Lv = 1u; Lv <= MaxBulletCountLv; ++Lv)
            {
                MainBulletCount.Add(Lv, new MainBulletCountData(Lv));
            }
        }
    }
}