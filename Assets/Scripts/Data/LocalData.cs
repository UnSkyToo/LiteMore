using System.Collections.Generic;

namespace LiteMore.Data
{
    public static class LocalData
    {
        public const int MaxWave = 1000;
        public const int MaxBulletLv = 100;

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
            MainBulletInterval.Clear();
            MainBulletCount.Clear();
            for (var Lv = 1u; Lv <= MaxBulletLv; ++Lv)
            {
                MainBulletDamage.Add(Lv, new MainBulletDamageData(Lv));
                MainBulletInterval.Add(Lv, new MainBulletIntervalData(Lv));
                MainBulletCount.Add(Lv, new MainBulletCountData(Lv));
            }
        }
    }
}