using System.Collections.Generic;
using System.Numerics;

namespace LiteFramework.Helper
{
    public static class BigNumHelper
    {
        private static readonly string[] SpecialSuffixList_ = new string[]
        {
            "K", "M", "B", "T"
        };

        private static readonly BigInteger CarryValue_ = new BigInteger(1000);
        private static readonly float CarryValueFloat_ = 1000.0f;

        private static readonly string[] LetterList_ = new string[26 * 26 + 4];
        private static readonly Dictionary<string, BigInteger> Letter2BigInt_ = new Dictionary<string, BigInteger>();

        static BigNumHelper()
        {
            Letter2BigInt_.Clear();

            var Val = BigInteger.One;
            var Carry = BigInteger.Parse("1000");
            var LetterIndex = 0;

            Val *= Carry;
            LetterList_[LetterIndex++] = "K";
            Letter2BigInt_.Add("K", Val);

            Val *= Carry;
            LetterList_[LetterIndex++] = "M";
            Letter2BigInt_.Add("M", Val);

            Val *= Carry;
            LetterList_[LetterIndex++] = "B";
            Letter2BigInt_.Add("B", Val);

            Val *= Carry;
            LetterList_[LetterIndex++] = "T";
            Letter2BigInt_.Add("T", Val);

            for (var C1 = 0; C1 < 26; ++C1)
            {
                for (var C2 = 0; C2 < 26; ++C2)
                {
                    Val *= Carry;
                    LetterList_[LetterIndex++] = $"{(char)(C1 + 'A')}{(char)(C2 + 'A')}";
                    Letter2BigInt_.Add($"{(char)(C1 + 'A')}{(char)(C2 + 'A')}", Val);
                }
            }
        }

        public static string ToLetter(BigInteger Value)
        {
            if (Value < CarryValue_)
            {
                return Value.ToString();
            }

            var NumLen = Value.ToString().Length;
            var CarryLen = NumLen % 3 == 0 ? (NumLen / 3) - 1 : (NumLen / 3);
            var Suffix = LetterList_[CarryLen - 1];

            if (CarryLen < 2)
            {
                var NumVal = (int)Value;
                var FloatVal = NumVal / CarryValueFloat_;

                return $"{FloatVal}{Suffix}";
            }
            else
            {
                var PrevSuffix = LetterList_[CarryLen - 2];
                var NumVal = (int)(Value / Letter2BigInt_[PrevSuffix]);
                var FloatVal = NumVal / CarryValueFloat_;

                return $"{FloatVal}{Suffix}";
            }
        }

        public static BigInteger ToBigInt(string Value)
        {
            if (IsDigitString(Value))
            {
                return BigInteger.Parse(Value);
            }

            if (Value.Length < 2 || !char.IsLetter(Value[Value.Length - 1]))
            {
                throw new LiteException($"error big value : {Value}");
            }

            Value = Value.ToUpper();
            var ValStr = string.Empty;
            var LetStr = string.Empty;
            if (char.IsLetter(Value[Value.Length - 2]))
            {
                ValStr = Value.Substring(0, Value.Length - 2);
                LetStr = Value.Substring(Value.Length - 2);
            }
            else
            {
                ValStr = Value.Substring(0, Value.Length - 1);
                LetStr = Value.Substring(Value.Length - 1);
            }

            var ValInt = new BigInteger(float.Parse(ValStr) * CarryValueFloat_);

            return ValInt * Letter2BigInt_[LetStr] / CarryValue_;
        }

        public static bool IsDigitString(string Value)
        {
            foreach (var Ch in Value)
            {
                if (!char.IsDigit(Ch))
                {
                    return false;
                }
            }

            return true;
        }
    }
}