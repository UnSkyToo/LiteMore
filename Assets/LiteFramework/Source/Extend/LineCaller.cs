using System;
using System.Collections.Generic;
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
            this.Width = 0.01f;
        }

        public LineCallerPoint(Vector2 Position, Color Color)
        {
            this.Position = Position;
            this.Color = Color;
            this.Width = 0.01f;
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

        public void Clear()
        {
            Renderer_.positionCount = 0;
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

        public void DrawPointList(Vector2[] PointList, Color StartColor, Color EndColor, float StartWidth, float EndWidth)
        {
            if (PointList == null || PointList.Length < 2)
            {
                return;
            }

            Renderer_.positionCount = PointList.Length;
            Renderer_.loop = false;

            for (var Index = 0; Index < PointList.Length; ++Index)
            {
                Renderer_.SetPosition(Index, PointList[Index]);
            }

            Renderer_.startColor = StartColor;
            Renderer_.startWidth = StartWidth;

            Renderer_.endColor = EndColor;
            Renderer_.endWidth = EndWidth;
        }

        public void DrawSmoothLine(ref Vector2[] PointList, Color LineColor, float Width)
        {
            var SmoothPointList = GetSmoothPointList(ref PointList);
            DrawPointList(SmoothPointList, LineColor, LineColor, Width, Width);
        }

        private static Vector2[] GetSmoothPointList(ref Vector2[] PointList, int Smooth = 10)
        {
            if (PointList.Length < 3)
            {
                return PointList;
            }

            var StartPoint = PointList[0];
            var SmoothAmount = PointList.Length * Smooth;
            PointList = PathControlPointGenerator(ref PointList);

            var Result = new Vector2[SmoothAmount];
            for (var Index = 1; Index <= SmoothAmount; ++Index)
            {
                Result[Index - 1] = Interpolation(ref PointList, (float)Index / SmoothAmount);
            }

            Result[0] = StartPoint;

            return Result;
        }

        private static Vector2[] PathControlPointGenerator(ref Vector2[] PointList)
        {
            var Result = new Vector2[PointList.Length + 2];
            Array.Copy(PointList, 0, Result, 1, PointList.Length);

            Result[0] = Result[1] + (Result[1] - Result[2]);
            Result[Result.Length - 1] = Result[Result.Length - 2] + (Result[Result.Length - 2] - Result[Result.Length - 3]);

            if (Result[1] == Result[Result.Length - 2])
            {
                var TempLoopSpline = new Vector2[Result.Length];
                Array.Copy(Result, TempLoopSpline, Result.Length);
                TempLoopSpline[0] = TempLoopSpline[TempLoopSpline.Length - 3];
                TempLoopSpline[TempLoopSpline.Length - 1] = TempLoopSpline[2];
                Result = new Vector2[TempLoopSpline.Length];
                Array.Copy(TempLoopSpline, Result, TempLoopSpline.Length);
            }

            return Result;
        }

        private static Vector2 Interpolation(ref Vector2[] PointList, float T)
        {
            var NumSections = PointList.Length - 3;
            var CurrentIndex = Mathf.Min(Mathf.FloorToInt(T * NumSections), NumSections - 1);
            var U = T * NumSections - CurrentIndex;

            var A = PointList[CurrentIndex];
            var B = PointList[CurrentIndex + 1];
            var C = PointList[CurrentIndex + 2];
            var D = PointList[CurrentIndex + 3];

            return 0.5f * (
                       (-A + 3f * B - 3f * C + D) * (U * U * U)
                       + (2f * A - 5f * B + 4f * C - D) * (U * U)
                       + (-A + C) * U
                       + 2f * B
                   );
        }
    }
}