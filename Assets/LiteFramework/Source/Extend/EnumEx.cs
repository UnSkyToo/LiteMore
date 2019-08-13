using System;

namespace LiteFramework.Extend
{
    public static class EnumEx
    {
        public static int Count<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}