using System;
using System.Collections.Generic;
using LiteFramework.Game.Asset;

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

        public static void Load(AssetUri Uri, Action<bool> Callback)
        {
            if (!DataList_.ContainsKey(Uri.AssetName))
            {
                var Data = new DataTable(Uri.AssetName);
                DataList_.Add(Uri.AssetName, Data);

                AssetManager.CreateDataAsync(Uri, Buffer =>
                {
                    var Succeeded = Data.Parse(Buffer);

                    if (!Succeeded)
                    {
                        DataList_.Remove(Uri.AssetName);
                    }

                    Callback?.Invoke(Succeeded);
                });
            }
        }

        public static DataTable GetTable(string DataName)
        {
            if (DataList_.ContainsKey(DataName))
            {
                return DataList_[DataName];
            }

            return null;
        }

        public static DataLine GetLine(string DataName, int ID)
        {
            if (DataList_.ContainsKey(DataName))
            {
                return DataList_[DataName].Line(ID);
            }

            return null;
        }

        public static T GetData<T>(string DataName, int ID, string Key)
        {
            if (DataList_.ContainsKey(DataName))
            {
                return DataList_[DataName].Line(ID).Get<T>(Key);
            }

            return default(T);
        }
    }
}