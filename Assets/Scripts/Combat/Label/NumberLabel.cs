using LiteMore.Motion;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Label
{
    public enum NumberLabelType : byte
    {
        Float,
        Bomb,
        Laser,
    }

    public class NumberLabel : BaseLabel
    {
        public NumberLabel(Transform Trans, NumberLabelType Type, float Value)
            : base("NumLabel", Trans)
        {
            Position = Trans.localPosition;

            InitWithType(Type, Value);
        }

        private void InitWithType(NumberLabelType Type, float Value)
        {
            var Txt = Transform_.GetComponent<Text>();
            Txt.text = $"{Value:0.0}";

            switch (Type)
            {
                case NumberLabelType.Float:
                    Txt.color = Color.red;
                    Txt.fontSize = 14;
                    break;
                case NumberLabelType.Bomb:
                    Txt.color = Color.yellow;
                    Txt.fontSize = 14;
                    break;
                case NumberLabelType.Laser:
                    Txt.color = Color.green;
                    Txt.fontSize = 30;
                    break;
                default:
                    break;
            }

            var Motion = CreateMotion(Type);
            if (Motion != null)
            {
                Transform_.ExecuteMotion(new SequenceMotion(Motion, new CallbackMotion(() => { IsAlive = false; })));
            }
            else
            {
                IsAlive = false;
            }
        }

        private BaseMotion CreateMotion(NumberLabelType Type)
        {
            switch (Type)
            {
                case NumberLabelType.Float:
                    return new MoveMotion(0.8f, new Vector3(0, 40, 0), true);
                case NumberLabelType.Bomb:
                    return new SequenceMotion(
                        new ScaleMotion(0.2f, new Vector3(2, 2, 1), false),
                        new WaitTimeMotion(0.5f));
                case NumberLabelType.Laser:
                    return new WaitTimeMotion(1);
                default:
                    return null;
            }
        }
    }
}