using System.Collections.Generic;

namespace LiteFramework.Game.Data
{
    public static class DataManager
    {
        private static readonly Dictionary<string, DataTable> DataList_ = new Dictionary<string, DataTable>();

        public static bool Startup()
        {
            DataList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
        }
    }
}