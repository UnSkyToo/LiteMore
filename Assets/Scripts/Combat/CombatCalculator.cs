namespace LiteMore.Combat
{
    public class CombatCalculator
    {
        private float TypePercent_;
        private float GlobalPercent_;

        public CombatCalculator()
        {
            TypePercent_ = 1.0f;
            GlobalPercent_ = 1.0f;
        }

        public float Calc(float Base, float Percent)
        {
            return Base * Percent * TypePercent_ * GlobalPercent_;
        }

        public void AddTypePercent(float Value)
        {
            TypePercent_ += Value;
        }

        public void AddGlobalPercent(float Value)
        {
            GlobalPercent_ += Value;
        }
    }
}