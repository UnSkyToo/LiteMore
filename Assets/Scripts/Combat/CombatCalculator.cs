using LiteMore.Extend;
using LiteMore.Player;

namespace LiteMore.Combat
{
    public class CombatCalculator
    {
        public float TypePercent { get; private set; }
        public float GlobalPercent { get; private set; }

        public CombatCalculator()
        {
            TypePercent = 1.0f;
            GlobalPercent = 1.0f;
        }

        public float Calc(float Base, float Percent)
        {
            return Base * Percent * TypePercent * GlobalPercent;
        }

        public void AddTypePercent(float Value)
        {
            TypePercent += Value;
        }

        public void AddGlobalPercent(float Value)
        {
            GlobalPercent += Value;
        }
    }
}