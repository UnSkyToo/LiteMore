namespace LiteMore
{
    public static class IDGenerator
    {
        private static uint ID_ = 0;

        public static uint Get()
        {
            ID_++;
            return ID_;
        }
    }
}