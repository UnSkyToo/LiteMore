using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public static class BezierCurveFactory
    {
        public static IBezierCurve CreateBezierCurve(Vector2 Begin, Vector2 End)
        {
            return new BezierCurveCommon(new[] {Begin, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector2 Begin,Vector2 Control, Vector2 End)
        {
            return new BezierCurveCommon(new[] {Begin, Control, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector2 Begin, Vector2 Control1, Vector2 Control2, Vector2 End)
        {
            return new BezierCurveCommon(new[] {Begin, Control1, Control2, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector2[] Points)
        {
            return new BezierCurveCommon(Points);
        }
    }
}