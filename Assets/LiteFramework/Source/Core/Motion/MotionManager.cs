using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public static class MotionManager
    {
        private static readonly ListEx<BaseMotion> MotionList_ = new ListEx<BaseMotion>();

        public static bool Startup()
        {
            MotionList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            MotionList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            foreach (var Motion in MotionList_)
            {
                if (Motion.IsEnd)
                {
                    Motion.Exit();
                    MotionList_.Remove(Motion);
                }
                else
                {
                    Motion.Tick(DeltaTime);
                }
            }
            MotionList_.Flush();
        }

        public static BaseMotion Execute(Transform Master, BaseMotion Motion)
        {
            if (Motion == null)
            {
                return null;
            }

            Motion.Master = Master;
            Motion.Enter();
            MotionList_.Add(Motion);
            return Motion;
        }

        public static void Abandon(BaseMotion Motion)
        {
            Motion?.Stop();
        }

        public static void Abandon(Transform Master)
        {
            if (Master == null)
            {
                return;
            }

            foreach (var Motion in MotionList_)
            {
                if (Motion.Master == Master)
                {
                    Abandon(Motion);
                }
            }
        }

        public static BaseMotion ExecuteMotion(this Transform Master, BaseMotion Motion)
        {
            return Execute(Master, Motion);
        }
    }
}