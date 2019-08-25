using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public interface IBezierCurve
    {
        Vector2 Lerp(float Time);
    }
}