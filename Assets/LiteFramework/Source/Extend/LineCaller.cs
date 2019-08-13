using UnityEngine;

namespace LiteFramework.Extend
{
    public struct LineCallerPoint
    {
        public Vector2 Position;
        public Color Color;
        public float Width;

        public LineCallerPoint(Vector2 Position)
        {
            this.Position = Position;
            this.Color = Color.white;
            this.Width = 1.0f;
        }

        public LineCallerPoint(Vector2 Position, Color Color)
        {
            this.Position = Position;
            this.Color = Color;
            this.Width = 1.0f;
        }

        public LineCallerPoint(Vector2 Position, Color Color, float Width)
        {
            this.Position = Position;
            this.Color = Color;
            this.Width = Width;
        }
    }
    
    public class LineCaller
    {
        private readonly LineRenderer Renderer_;

        public LineCaller(LineRenderer Renderer)
        {
            Renderer_ = Renderer;
        }

        public void DrawLine(LineCallerPoint Begin, LineCallerPoint End)
        {
            Renderer_.positionCount = 2;
            Renderer_.loop = false;

            Renderer_.SetPosition(0, Begin.Position);
            Renderer_.startColor = Begin.Color;
            Renderer_.startWidth = Begin.Width;

            Renderer_.SetPosition(1, End.Position);
            Renderer_.endColor = End.Color;
            Renderer_.endWidth = End.Width;
        }

        public void DrawRect(LineCallerPoint LeftTop, LineCallerPoint RightBottom)
        {
            Renderer_.positionCount = 4;
            Renderer_.loop = true;

            Renderer_.SetPosition(0, LeftTop.Position);
            Renderer_.startColor = LeftTop.Color;
            Renderer_.startWidth = LeftTop.Width;

            Renderer_.SetPosition(1, new Vector3(RightBottom.Position.x, LeftTop.Position.y, 0));

            Renderer_.SetPosition(2, RightBottom.Position);
            Renderer_.endColor = RightBottom.Color;
            Renderer_.endWidth = RightBottom.Width;

            Renderer_.SetPosition(3, new Vector3(LeftTop.Position.x, RightBottom.Position.y, 0));
        }

        public void DrawCircle(LineCallerPoint Center, float Radius, int Precision = 50)
        {
            Renderer_.positionCount = Precision;
            Renderer_.loop = true;

            for (var Index = 0; Index < Precision; ++Index)
            {
                var Angle = (float)Index / (float)Precision * Mathf.PI * 2;
                var X = Mathf.Sin(Angle) * Radius + Center.Position.x;
                var Y = Mathf.Cos(Angle) * Radius + Center.Position.y;
                Renderer_.SetPosition(Index, new Vector3(X, Y, 0));
            }

            Renderer_.startColor = Renderer_.endColor = Center.Color;
            Renderer_.startWidth = Renderer_.endWidth = Center.Width;
        }
    }
}