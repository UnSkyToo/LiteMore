using System.Collections.Generic;
using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public static class MotionManager
    {
        private static readonly ListEx<BaseMotion> MotionList_ = new ListEx<BaseMotion>();
        private static float DeltaTime_ = 0;

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
            DeltaTime_ = DeltaTime;
            MotionList_.Foreach((Entity) =>
            {
                if (Entity.Master == null)
                {
                    MotionList_.Remove(Entity);
                }
                else if (Entity.IsEnd)
                {
                    Entity.Exit();
                    MotionList_.Remove(Entity);
                }
                else
                {
                    Entity.Tick(DeltaTime_);
                }
            });
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

        public static List<BaseMotion> GetMotion(Transform Master)
        {
            var Result = new List<BaseMotion>();

            foreach (var Motion in MotionList_)
            {
                if (!Motion.IsEnd && Motion.Master == Master)
                {
                    Result.Add(Motion);
                }
            }

            return Result;
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

            var MotionList = GetMotion(Master);
            foreach (var Motion in MotionList)
            {
                Abandon(Motion);
            }
        }

        public static BaseMotion ExecuteMotion(this Transform Master, BaseMotion Motion)
        {
            return Execute(Master, Motion);
        }

        public static void AbandonMotion(this Transform Master)
        {
            Abandon(Master);
        }

        public static bool IsExecute(Transform Master)
        {
            return GetMotion(Master).Count > 0;
        }

        public static bool HasExecuteMotion(this Transform Master)
        {
            return IsExecute(Master);
        }
    }
}