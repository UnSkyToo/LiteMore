using System.Collections.Generic;
using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat
{
    public class BaseSfx : GameEntity
    {
        protected Animator Animator_;

        public BaseSfx(Transform Trans)
            : base(Trans)
        {
            Animator_ = Transform_.GetComponentInChildren<Animator>();
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            if (IsEnd())
            {
                IsAlive = false;
            }
        }

        public bool IsEnd()
        {
            var Info = Animator_.GetCurrentAnimatorStateInfo(0);
            return Info.normalizedTime >= 1.0f;
        }
    }

    public static class SfxManager
    {
        private static readonly List<BaseSfx> SfxList_ = new List<BaseSfx>();

        public static bool Startup()
        {
            SfxList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in SfxList_)
            {
                Entity.Dispose();
            }
            SfxList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = SfxList_.Count - 1; Index >= 0; --Index)
            {
                SfxList_[Index].Tick(DeltaTime);

                if (!SfxList_[Index].IsAlive)
                {
                    SfxList_[Index].Dispose();
                    SfxList_.RemoveAt(Index);
                }
            }
        }

        public static BaseSfx AddSfx(string ResName, Vector2 Position)
        {
            var Obj = Object.Instantiate(Resources.Load<GameObject>(ResName));
            Obj.transform.SetParent(Configure.SfxRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new BaseSfx(Obj.transform);
            SfxList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }
    }
}