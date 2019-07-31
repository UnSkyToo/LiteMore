using UnityEngine;

namespace LiteMore.Data
{
    public class WaveData
    {
        public uint Wave { get; }
        public int TriggerCount { get; }
        public uint EmitterCount { get; }
        public float Interval { get; }
        public float Speed { get; }
        public float Hp { get; }
        public float Damage { get; }
        public int Gem { get; }

        public WaveData(uint Wave)
        {
            this.Wave = Wave;
            this.TriggerCount = Formula_TriggerCount(Wave);
            this.EmitterCount = Formula_EmitterCount(Wave);
            this.Interval = Formula_Interval(Wave);
            this.Speed = Formula_Speed(Wave);
            this.Hp = Formula_Hp(Wave);
            this.Damage = Formula_Damage(Wave);
            this.Gem = Formula_Gem(Wave);
        }

        public static int Formula_TriggerCount(uint Wave)
        {
            return (int)(4 + (Wave - 1) * 0.04f);
        }

        public static uint Formula_EmitterCount(uint Wave)
        {
            return 5 + (uint)Mathf.Pow(Wave, 0.8045f);
        }

        public static float Formula_Interval(uint Wave)
        {
            return 8;
        }

        public static float Formula_Speed(uint Wave)
        {
            return 50;
        }

        public static float Formula_Hp(uint Wave)
        {
            return 5 + (Wave - 1) * 0.475f;
        }

        public static float Formula_Damage(uint Wave)
        {
            return 1;
        }

        public static int Formula_Gem(uint Wave)
        {
            return 1 + (int)((Wave - 1) * 0.1f);
        }
    }
}