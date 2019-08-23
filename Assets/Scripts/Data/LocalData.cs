using System.Collections.Generic;
using LiteMore.Combat.Buff;
using LiteMore.Combat.Skill;

namespace LiteMore.Data
{
    public static class LocalData
    {
        private const int MaxWaveLv = 200;

        public static Dictionary<uint, WaveData> WaveList { get; private set; }
        public static MainBulletData MainBullet { get; private set; }
        public static Dictionary<uint, SkillData> SkillList { get; private set; }

        public static void Generate()
        {
            WaveList = new Dictionary<uint, WaveData>();
            for (var Wave = 1u; Wave <= MaxWaveLv; ++Wave)
            {
                WaveList.Add(Wave, new WaveData(Wave));
            }

            MainBullet = new MainBulletData(MaxWaveLv);

            SkillList = new Dictionary<uint, SkillData>();
            SkillLibrary.Generate();
            BuffLibrary.Generate();
        }
    }
}