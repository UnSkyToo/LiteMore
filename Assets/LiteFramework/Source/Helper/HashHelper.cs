namespace LiteFramework.Helper
{
    public static class HashHelper
    {
        public static int Combine(int Value1, int Value2)
        {
            var Rol5 = ((uint)Value1 << 5) | ((uint)Value1 >> 27);
            return ((int)Rol5 + Value1) ^ Value2;
        }

        public static int Combine(int Value1, int Value2, int Value3)
        {
            return Combine(Combine(Value1, Value2), Value3);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4)
        {
            return Combine(Combine(Combine(Value1, Value2), Value3), Value4);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5)
        {
            return Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5, int Value6)
        {
            return Combine(Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5), Value6);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5, int Value6, int Value7)
        {
            return Combine(Combine(Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5), Value6), Value7);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5, int Value6, int Value7, int Value8)
        {
            return Combine(Combine(Combine(Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5), Value6), Value7), Value8);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5, int Value6, int Value7, int Value8, int Value9)
        {
            return Combine(Combine(Combine(Combine(Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5), Value6), Value7), Value8), Value9);
        }

        public static int Combine(int Value1, int Value2, int Value3, int Value4, int Value5, int Value6, int Value7, int Value8, int Value9, int Value10)
        {
            return Combine(Combine(Combine(Combine(Combine(Combine(Combine(Combine(Combine(Value1, Value2), Value3), Value4), Value5), Value6), Value7), Value8), Value9), Value10);
        }

        public static int Array<T>(T[] Items)
        {
            if (Items == null || Items.Length == 0)
            {
                return 0;
            }

            var Hash = Items[0].GetHashCode();
            for (var Index = 1; Index < Items.Length; ++Index)
            {
                Hash = Combine(Hash, Items[Index]?.GetHashCode() ?? Index);
            }

            return Hash;
        }
    }
}