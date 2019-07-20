using UnityEngine;

namespace LiteMore.Extend
{
    public class Fps : MonoBehaviour
    {
        public bool Enable { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Color TextColor { get; set; }

        private int FpsCount_ = 60;
        private int CurrentFps_ = 0;
        private float FpsTime_ = 0.0f;
        private GUIStyle Style_;

        void Awake()
        {
            Enable = true;
            X = 5;
            Y = 5;
            TextColor = Color.green;

            Style_ = new GUIStyle {fontSize = 40, normal = {background = null, textColor = TextColor}};
        }

        void Update()
        {
            FpsTime_ += Time.deltaTime;

            if (FpsTime_ >= 1.0f)
            {
                FpsTime_ -= 1.0f;
                FpsCount_ = CurrentFps_;
                CurrentFps_ = 0;
            }

            CurrentFps_++;
        }

        void OnGUI()
        {
            if (Enable)
            {
                GUI.Label(new Rect(X, Y, 100, 40), $"Fps:{FpsCount_}({(Time.deltaTime * 1000):0.00}ms)", Style_);
            }
        }
    }
}