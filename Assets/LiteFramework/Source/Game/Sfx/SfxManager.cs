using System;
using LiteFramework.Core.Base;
using LiteFramework.Game.Asset;
using LiteFramework.Helper;
using UnityEngine;

namespace LiteFramework.Game.Sfx
{
    public static class SfxManager
    {
        private static readonly ListEx<BaseSfx> SfxList_ = new ListEx<BaseSfx>();

        public static bool Startup()
        {
            SfxList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            SfxList_.Foreach((Entity) =>
            {
                Entity.Dispose();
            });
            SfxList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            SfxList_.Foreach((Entity, Time) =>
            {
                Entity.Tick(Time);

                if (!Entity.IsAlive)
                {
                    Entity.Dispose();
                    SfxList_.Remove(Entity);
                }
            }, DeltaTime);
        }

        public static SkeletonSfx PlaySkeletonSfx(Transform Parent, AssetUri Uri, string AnimationName = "", bool IsLoop = false, Action Finished = null)
        {
            if (Parent == null || Uri == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(AnimationName))
            {
                AnimationName = Uri.AssetName;
            }

            var Obj = AssetManager.CreatePrefabSync(Uri);
            var Sfx = new SkeletonSfx(Uri.AssetName, Obj.transform);
            SfxList_.Add(Sfx);
            Sfx.SetParent(Parent, false);
            Sfx.Play(AnimationName, IsLoop, Finished);
            Sfx.Position = Vector2.zero;
            return Sfx;
        }

        public static void PlaySkeletonSfxAsync(Transform Parent, AssetUri Uri, Vector2 Position, Action<SkeletonSfx> Callback, string AnimationName = "", bool IsLoop = false, Action Finished = null)
        {
            if (Parent == null || Uri == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(AnimationName))
            {
                AnimationName = Uri.AssetName;
            }
            
            AssetManager.CreatePrefabAsync(Uri, (Obj) =>
            {
                var Sfx = new SkeletonSfx(Uri.AssetName, Obj.transform);
                SfxList_.Add(Sfx);
                Sfx.SetParent(Parent, false);
                Sfx.Play(AnimationName, IsLoop, Finished);
                Sfx.Position = Position;
                Callback?.Invoke(Sfx);
            });
        }

        public static ParticleSfx PlayParticleSfx(Transform Parent, AssetUri Uri, bool IsLoop = false, Action Finished = null)
        {
            if (Parent == null || Uri == null)
            {
                return null;
            }

            var Obj = AssetManager.CreatePrefabSync(Uri);
            var Sfx = new ParticleSfx(Uri.AssetName, Obj.transform);
            SfxList_.Add(Sfx);
            Sfx.SetParent(Parent, false);
            Sfx.Play(string.Empty, IsLoop, Finished);
            Sfx.Position = Vector2.zero;

            var Canvas = UnityHelper.GetComponentUpper<Canvas>(Parent);
            if (Canvas != null && Canvas.overrideSorting)
            {
                UnityHelper.AddSortingOrder(Obj, Canvas.sortingOrder + 1);
            }

            return Sfx;
        }

        public static void PlayParticleSfxAsync(Transform Parent, AssetUri Uri, Vector2 Position, Action<ParticleSfx> Callback, bool IsLoop = false, Action Finished = null)
        {
            if (Parent == null || Uri == null)
            {
                return;
            }

            AssetManager.CreatePrefabAsync(Uri, (Obj) =>
            {
                var Sfx = new ParticleSfx(Uri.AssetName, Obj.transform);
                SfxList_.Add(Sfx);
                Sfx.SetParent(Parent, false);
                Sfx.Play(string.Empty, IsLoop, Finished);
                Sfx.Position = Position;

                var Canvas = UnityHelper.GetComponentUpper<Canvas>(Parent);
                if (Canvas != null && Canvas.overrideSorting)
                {
                    UnityHelper.AddSortingOrder(Obj, Canvas.sortingOrder + 1);
                }

                Callback?.Invoke(Sfx);
            });
        }

        public static void StopSfx(BaseSfx Sfx)
        {
            Sfx?.Stop();
        }
    }
}