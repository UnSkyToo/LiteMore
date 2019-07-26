using System.Collections.Generic;

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
            Text_["Hp"] = "防御值";
            Text_["Mp"] = "能量值";
            Text_["Gem"] = "宝石";
            Text_["Wave"] = "波数";
            Text_["RemainNum"] = "剩余数量";
        }

        private static void LoadEN()
        {
        }
    }
}