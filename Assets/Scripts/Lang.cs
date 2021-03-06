﻿using System.Collections.Generic;

namespace LiteMore
{
    public static class Lang
    {
        private static readonly Dictionary<string, string> Text_ = new Dictionary<string, string>();

        public static string Localize(this string Key)
        {
            return Get(Key);
        }

        public static string Get(string Key)
        {
            if (Text_.ContainsKey(Key))
            {
                return Text_[Key];
            }

            return Key;
        }

        public static void Load()
        {
            Text_.Clear();
            LoadZH_CN();
        }

        private static void LoadZH_CN()
        {
        }

        private static void LoadEN()
        {
        }
    }
}