using System.Collections.Generic;
using UnityEngine;

namespace LiteMore
{
    public class Sfx : EntityBase
    {
        private Transform Transform_;
        protected Animator Animator_;

        public Sfx(Transform Trans)
            : base()
        {
            Transform_ = Trans;
            Transform_.name = $"<{ID}>";

            Animator_ = Trans.GetComponentInChildren<Animator>();
        }

        public override void Create()
        {
        }

        public override void Destroy()
        {
            IsAlive = false;

            if (Transform_ != null)
            {
                Object.Destroy(Transform_.gameObject);
                Transform_ = null;
            }
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            Transform_.localPosition = Position;

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
        private static Transform SfxRoot_;
        private static List<Sfx> SfxList_;

        public static bool Startup()
        {
            SfxRoot_ = GameObject.Find("Sfx").transform;
            SfxList_ = new List<Sfx>();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in SfxList_)
            {
                Entity.Destroy();
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
                    SfxList_[Index].Destroy();
                    SfxList_.RemoveAt(Index);
                }
            }
        }

        public static Sfx AddSfx(string ResName, Vector2 Position)
        {
            var Obj = Object.Instantiate(Resources.Load<GameObject>(ResName));
            Obj.transform.SetParent(SfxRoot_, false);
            Obj.transform.localPosition = Position;

            var Entity = new Sfx(Obj.transform);
            Entity.Create();
            SfxList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }
    }
}