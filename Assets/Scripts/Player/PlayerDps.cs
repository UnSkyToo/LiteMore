using System.Collections.Generic;

namespace LiteMore.Player
{
    public class PlayerDps
    {
        public class PlayerDpsChunk
        {
            public string SourceName { get; }
            public float Value { get; }
            public float Percent { get; }

            public PlayerDpsChunk(string SourceName, float Value, float Percent)
            {
                this.SourceName = SourceName;
                this.Value = Value;
                this.Percent = Percent;
            }
        }

        public float Dps { get; private set; }
        private readonly float Interval_;
        private float TotalValue_;
        private float CurrentValue_;
        private float Time_;

        private readonly Dictionary<string, float> ChunkList_;

        public PlayerDps(float Interval = 1.0f)
        {
            Dps = 0;
            Interval_ = Interval;
            CurrentValue_ = TotalValue_ = 0;
            Time_ = 0;

            ChunkList_ = new Dictionary<string, float>();
        }

        public void Tick(float DeltaTime)
        {
            Time_ += DeltaTime;
            if (Time_ >= Interval_)
            {
                Time_ -= Interval_;
                Dps = CurrentValue_ / Interval_;
                CurrentValue_ = 0;
            }
        }

        public void ApplyDamage(string SourceName, float Value)
        {
            TotalValue_ += Value;
            CurrentValue_ += Value;

            if (!ChunkList_.ContainsKey(SourceName))
            {
                ChunkList_.Add(SourceName, Value);
            }
            else
            {
                ChunkList_[SourceName] += Value;
            }
        }

        public void Clear()
        {
            TotalValue_ = 0;
            ChunkList_.Clear();
        }

        public List<PlayerDpsChunk> GetChunks()
        {
            var Chunks = new List<PlayerDpsChunk>();

            foreach (var Chunk in ChunkList_)
            {
                Chunks.Add(new PlayerDpsChunk(Chunk.Key, Chunk.Value, Chunk.Value / TotalValue_));
            }

            Chunks.Sort((A, B) =>
            {
                if (A.Percent < B.Percent)
                {
                    return 1;
                }

                if (A.Percent > B.Percent)
                {
                    return -1;
                }

                return 0;
            });

            return Chunks;
        }
    }
}