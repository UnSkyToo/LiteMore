using System;

namespace LiteMore.Extend
{
    public static class EnumEx
    {
        public static int Count<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}