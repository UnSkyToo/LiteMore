using LiteMore.UI;

namespace LiteMore.Combat.Wave
{
    public static class WaveManager
    {
        private static BaseWave CurrentWave_;

        public static bool Startup()
        {
            LoadWave(1);

            return true;
        }

        public static void Shutdown()
        {
            CurrentWave_?.Dispose();
        }

        public static void Tick(float DeltaTime)
        {
            if (CurrentWave_ != null)
            {
                CurrentWave_.Tick(DeltaTime);

                if (!CurrentWave_.IsAlive)
                {
                    LoadWave(CurrentWave_.Wave + 1);
                }
            }
        }

        public static void LoadWave(uint Wave)
        {
            CurrentWave_?.Dispose();
            CurrentWave_ = new BaseWave(Wave);

            EventManager.Send<WaveChangeEvent>();
            EventManager.Send<NewWaveEvent>();
        }

        public static BaseWave GetWave()
        {
            return CurrentWave_;
        }
    }
}