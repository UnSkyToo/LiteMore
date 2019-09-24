namespace LiteFramework.Game.Data
{
    public abstract class BaseDataEntity
    {
    }

    public class DataEntity<T> : BaseDataEntity
    {
        private readonly T Value_;

        public DataEntity(T Value)
        {
            Value_ = Value;
        }

        public T Get()
        {
            return Value_;
        }
    }
}
