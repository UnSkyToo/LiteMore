using UnityEngine;

namespace LiteMore.Helper
{
    public static class MathHelper
    {
        public static float GetAngle(Vector3 From, Vector3 To)
        {
            var Offset = To - From;
            return GetAngle(Offset);
        }

        public static float GetAngle(Vector3 Offset)
        {
            var Angle = Mathf.Atan2(Offset.x, Offset.y);
            return ((Angle / Mathf.PI * 180.0f) + 360.0f) % 360.0f;
        }

        /// <summary>
        /// GetAngle是顺时针0-360，GetUnityAngle是逆时针0-360
        /// </summary>
        public static float GetUnityAngle(Vector3 From, Vector3 To)
        {
            return -GetAngle(From, To);
        }

        /// <summary>
        /// GetAngle是顺时针0-360，GetUnityAngle是逆时针0-360
        /// </summary>
        public static float GetUnityAngle(Vector3 Offset)
        {
            return -GetAngle(Offset);
        }

        public static float CalcYaw(Vector3 Direction)
        {
            Direction.Normalize();

            var Yaw = Vector3.Dot(Direction, Vector3.up);
            Yaw = Mathf.Clamp(Yaw, -1.0f, 1.0f);
            Yaw = Mathf.Acos(Yaw);

            if (Direction.x < 0)
            {
                Yaw = Mathf.PI * 2.0f - Yaw;
            }

            return Yaw;
        }
    }
}