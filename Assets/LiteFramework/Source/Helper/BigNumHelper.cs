using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class BigNumHelper
    {
        private static readonly BigInteger CarryValue_ = new BigInteger(1000);
        private static readonly float CarryValueFloat_ = 1000.0f;
        private static readonly BigInteger MaxMulValue_ = new BigInteger(1000000);

        private static readonly string[] LetterList_ = new string[26 * 26 + 4];
        private static readonly Dictionary<string, BigInteger> Letter2BigInt_ = new Dictionary<string, BigInteger>();
        private static readonly Regex RegRule = new Regex(@"^(\d+(\.\d+)?)([A-Za-z]{0,2})$");

        static BigNumHelper()
        {
            Letter2BigInt_.Clear();

            var Val = BigInteger.One;
            var LetterIndex = 0;

            Val *= CarryValue_;
            LetterList_[LetterIndex++] = "K";
            Letter2BigInt_.Add("K", Val);

            Val *= CarryValue_;
            LetterList_[LetterIndex++] = "M";
            Letter2BigInt_.Add("M", Val);

            Val *= CarryValue_;
            LetterList_[LetterIndex++] = "B";
            Letter2BigInt_.Add("B", Val);

            Val *= CarryValue_;
            LetterList_[LetterIndex++] = "T";
            Letter2BigInt_.Add("T", Val);

            for (var C1 = 0; C1 < 26; ++C1)
            {
                for (var C2 = 0; C2 < 26; ++C2)
                {
                    Val *= CarryValue_;
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

        public static bool TryToBigInt(string Value, out BigInteger Result)
        {
            var (FloatVal, LetStr) = ParseLetterRegex(Value);
            if (string.IsNullOrWhiteSpace(LetStr))
            {
                return BigInteger.TryParse(Value, out Result);
            }

            var ValInt = new BigInteger(FloatVal * CarryValueFloat_);
            Result = ValInt * Letter2BigInt_[LetStr] / CarryValue_;
            return true;
        }

        public static BigInteger ToBigInt(string Value)
        {
            /*if (IsDigitString(Value))
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
            }*/

            var (FloatVal, LetStr) = ParseLetterRegex(Value);
            if (string.IsNullOrWhiteSpace(LetStr))
            {
                return BigInteger.Parse(Value);
            }

            var ValInt = new BigInteger(FloatVal * CarryValueFloat_);

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

        public static (float, string) ParseLetterRegex(string Source)
        {
            var Match = RegRule.Match(Source);
            return Match.Success
                ? (float.Parse(Match.Groups[1].Value), Match.Groups[3].Value)
                : (0, string.Empty);
        }

        public static BigInteger Mul(BigInteger Base, float Factor)
        {
            if (Factor < CarryValueFloat_ && Base < MaxMulValue_)
            {
                return (int)((int) Base * Factor);
            }

            var Result = Base * (int)Factor;
            Result += ((Base / CarryValue_) * (int)(Mathf.Abs((int)Factor - Factor) * CarryValueFloat_));
            return Result;
        }

        public static float DivPercent(BigInteger Dividend, BigInteger Divisor)
        {
            var Factor = BigInteger.Divide(Dividend * 100, Divisor);
            return Mathf.Clamp01(((float) Factor) * 0.01f);
        }
    }
}