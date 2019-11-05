using System;

namespace LiteFramework.Extend
{
    public static class EnumEx
    {
        public static int Count<T>()
        {
            return Enum.GetNames(typeof(T)).Length;
        }
    }
}