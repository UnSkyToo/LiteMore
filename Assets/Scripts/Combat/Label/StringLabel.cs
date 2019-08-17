using LiteFramework.Core.Motion;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Label
{
    public class StringLabel : BaseLabel
    {
        public StringLabel(Transform Trans, string Value, Color TextColor, int FontSize)
            : base("StringLabel", Trans)
        {
            Position = Trans.localPosition;

            InitWithType(Value, TextColor, FontSize);
        }

        private void InitWithType(string Value, Color TextColor, int FontSize)
        {
            var Txt = Transform_.GetComponent<Text>();
            Txt.text = Value;
            Txt.color = TextColor;
            Txt.fontSize = FontSize;
            Txt.fontStyle = FontStyle.Bold;

            var Motion = new MoveMotion(0.8f, new Vector3(0, 40, 0), true);
            Transform_.ExecuteMotion(new SequenceMotion(Motion, new CallbackMotion(() => { IsAlive = false; })));
        }
    }
}