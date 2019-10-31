using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Npc;
using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat
{
    public class BaseSfx : GameEntity
    {
        protected Animator Animator_;

        public BaseSfx(string Name, Transform Trans)
            : base(Name, Trans)
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
            if (!IsAlive)
            {
                return true;
            }

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

        private static BaseSfx CreateSfx(string ResName)
        {
            var Obj = AssetManager.CreatePrefabSync(new AssetUri(ResName));
            var Entity = new BaseSfx(ResName, Obj.transform);
            SfxList_.Add(Entity);
            return Entity;
        }

        public static BaseSfx PlayNpcSfx(BaseNpc Master, bool IsFront, string ResName)
        {
            var Sfx = CreateSfx(ResName);
            Sfx.Entity.SetParent(IsFront ? Master.FrontLayer : Master.BackLayer, false);
            return Sfx;
        }

        public static BaseSfx PlayGroundSfx(Vector2 Position, string ResName)
        {
            var Sfx = CreateSfx(ResName);
            MapManager.AddToGroundLayer(Sfx.Entity);
            Sfx.Position = Position;
            return Sfx;
        }

        public static BaseSfx PlaySkySfx(Vector2 Position, string ResName)
        {
            var Sfx = CreateSfx(ResName);
            MapManager.AddToSkyLayer(Sfx.Entity);
            Sfx.Position = Position;
            return Sfx;
        }
    }
}