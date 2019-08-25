using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public class BezierCurveCommon : IBezierCurve
    {
        private readonly Vector2[] Points_;
        private readonly Vector2[] Controller_;

        public BezierCurveCommon(Vector2[] Points)
        {
            if (Points != null && Points.Length > 1)
            {
                Points_ = Points;
                Controller_ = new Vector2[Points.Length];

                for (var Index = 0; Index < Controller_.Length; ++Index)
                {
                    Controller_[Index] = new Vector2();
                }
            }
        }

        public Vector2 Lerp(float Time)
        {
            if (Points_ == null)
            {
                return Vector2.zero;
            }

            var Count = GeneratorController(Points_, Points_.Length, Time);
            while (Count > 1)
            {
                Count = GeneratorController(Controller_, Count, Time);
            }
            return Controller_[0];
        }

        private int GeneratorController(Vector2[] Points, int Count, float Time)
        {
            var Index = 0;
            for (var Offset = 0; Offset < Count - 1; ++Offset)
            {
                Controller_[Index].x = (Points[Offset + 1].x - Points[Offset].x) * Time + Points[Offset].x;
                Controller_[Index].y = (Points[Offset + 1].y - Points[Offset].y) * Time + Points[Offset].y;
                Index++;
            }
            return Index;
        }
    }
}