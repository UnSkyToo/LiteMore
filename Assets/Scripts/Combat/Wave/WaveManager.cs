using LiteMore.UI;
using LiteMore.UI.Logic;

namespace LiteMore.Combat.Wave
{
    public static class WaveManager
    {
        private static BaseWave CurrentWave_;

        public static bool Startup()
        {
            UIManager.OpenUI<WaveInfoUI>();

            LoadWave(1);

            return true;
        }

        public static void Shutdown()
        {
            UIManager.CloseUI<WaveInfoUI>();
            CurrentWave_?.Destroy();
        }

        public static void Tick(float DeltaTime)
        {
            if (CurrentWave_ != null)
            {
                CurrentWave_.Tick(DeltaTime);

                if (CurrentWave_.IsEnd)
                {
                    LoadWave(CurrentWave_.Wave + 1);
                }
            }
        }

        public static void LoadWave(uint Wave)
        {
            CurrentWave_?.Destroy();
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