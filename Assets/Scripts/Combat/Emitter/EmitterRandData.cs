using UnityEngine;

namespace LiteMore.Combat.Emitter
{
    public class EmitterRandData<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        private readonly System.Func<T, T, T> CreateFunc_;

        public EmitterRandData(System.Func<T, T, T> CreateFunc)
        {
            Min = default;
            Max = default;
            CreateFunc_ = CreateFunc;
        }

        public T Get()
        {
            return CreateFunc_(Min, Max);
        }
    }

    public class EmitterRandFloat : EmitterRandData<float>
    {
        public EmitterRandFloat(float Min, float Max)
            : base(Random.Range)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public class EmitterRandInt : EmitterRandData<int>
    {
        public EmitterRandInt(int Min, int Max)
            : base(Random.Range)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public class EmitterRandVector2 : EmitterRandData<Vector2>
    {
        public EmitterRandVector2(Vector2 Min, Vector2 Max)
            : base((A, B) => new Vector2(Random.Range(A.x, B.x), Random.Range(A.y, B.y)))
        {
            this.Min = Min;
            this.Max = Max;
        }
    }
}