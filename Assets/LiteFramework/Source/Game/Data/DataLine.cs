using System.Collections.Generic;

namespace LiteFramework.Game.Data
{
    public class DataLine
    {
        private readonly Dictionary<string, BaseDataEntity> DataList_ = null;

        public DataLine()
        {
            DataList_ = new Dictionary<string, BaseDataEntity>();
        }

        public void Add(string Key, BaseDataEntity Value)
        {
            if (DataList_.ContainsKey(Key))
            {
                throw new LiteException($"repeat key : {Key}");
            }

            DataList_.Add(Key, Value);
        }

        public T Get<T>(string Key)
        {
            if (!DataList_.ContainsKey(Key))
            {
                throw new LiteException($"can't find key {Key}");
            }

            if (DataList_[Key] is DataEntity<T> Data)
            {
                return Data.Get();
            }

            throw new LiteException($"unexpected type : {typeof(T)} in {Key}");
        }
    }
}