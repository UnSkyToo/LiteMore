﻿using LiteMore.Cache;

namespace LiteMore.Player
{
    public class PlayerInfo
    {
        public float Hp { get; set; }
        public float MaxHp { get; set; }
        public float AddHp { get; set; }

        public float Mp { get; set; }
        public float MaxMp { get; set; }
        public float AddMp { get; set; }

        public int Gem { get; set; }
        public int Wave { get; set; }

        public uint BulletDamageLevel { get; set; }
        public uint BulletIntervalLevel { get; set; }
        public uint BulletCountLevel { get; set; }

        private bool IgnoreSaveCache_;

        public PlayerInfo()
        {
            IgnoreSaveCache_ = false;
        }

        public bool DeleteArchive()
        {
            IgnoreSaveCache_ = LocalCache.ClearCache();
            return IgnoreSaveCache_;
        }

        public void LoadFromCache()
        {
            MaxHp = LocalCache.GetFloat(nameof(MaxHp), 100);
            AddHp = LocalCache.GetFloat(nameof(AddHp), 1);
            Hp = MaxHp;

            MaxMp = LocalCache.GetFloat(nameof(MaxMp), 100);
            AddMp = LocalCache.GetFloat(nameof(AddMp), 10);
            Mp = MaxMp;

            Gem = LocalCache.GetInt(nameof(Gem), 0);
            Wave = LocalCache.GetInt(nameof(Wave), 1);

            BulletDamageLevel = (uint)LocalCache.GetInt(nameof(BulletDamageLevel), 1);
            BulletIntervalLevel = (uint)LocalCache.GetInt(nameof(BulletIntervalLevel), 1);
            BulletCountLevel = (uint)LocalCache.GetInt(nameof(BulletCountLevel), 1);
        }

        public void SaveToCache()
        {
            if (IgnoreSaveCache_)
            {
                return;
            }

            LocalCache.SetFloat(nameof(MaxHp), MaxHp);
            LocalCache.SetFloat(nameof(AddHp), AddHp);

            LocalCache.SetFloat(nameof(MaxMp), MaxMp);
            LocalCache.SetFloat(nameof(AddMp), AddMp);

            LocalCache.SetInt(nameof(Gem), Gem);
            LocalCache.SetInt(nameof(Wave), Wave);

            LocalCache.SetInt(nameof(BulletDamageLevel), (int)BulletDamageLevel);
            LocalCache.SetInt(nameof(BulletIntervalLevel), (int)BulletIntervalLevel);
            LocalCache.SetInt(nameof(BulletCountLevel), (int)BulletCountLevel);
        }
    }
}