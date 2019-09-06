using System.Collections.Generic;
using LiteFramework.Interface;

namespace LiteFramework.Helper
{
    public static class LiteHelper
    {
        public static void SortPriority<T>(this List<T> Array) where T : IPriority
        {
            SortPriority<T>(ref Array);
        }

        public static void SortPriority<T>(ref List<T> Array) where T : IPriority
        {
            Array.Sort((A, B) =>
            {
                if (A.Priority > B.Priority)
                {
                    return -1;
                }

                if (A.Priority < B.Priority)
                {
                    return 1;
                }

                return 0;
            });
        }
    }
}