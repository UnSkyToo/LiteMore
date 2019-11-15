using System;
using LiteFramework.Core.Log;
using Spine.Unity;
using UnityEngine;

namespace LiteFramework.Game.Sfx
{
    public class SkeletonSfx : BaseSfx
    {
        private readonly SkeletonGraphic Graphic_;

        public SkeletonSfx(string Name, Transform Trans)
            : base(Name, Trans)
        {
            Graphic_ = GetComponent<SkeletonGraphic>();
            Graphic_.Initialize(false);
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            if (Graphic_.AnimationState.GetCurrent(0).Loop)
            {
                return;
            }

            IsAlive = !IsEnd();
        }

        public override void Dispose()
        {
            Graphic_.Initialize(false);
            Graphic_.AnimationState.ClearTracks();

            base.Dispose();
        }

        public override bool IsEnd()
        {
            return Graphic_.AnimationState.GetCurrent(0).IsComplete;
        }

        public override void Play(string AnimationName, bool IsLoop = false, Action Finished = null)
        {
            if (Graphic_.SkeletonData.FindAnimation(AnimationName) == null)
            {
                LLogger.LError($"can't play animation '{AnimationName}'");
                return;
            }

            Graphic_.AnimationState.SetAnimation(0, AnimationName, IsLoop);

            if (Finished == null)
            {
                Graphic_.AnimationState.Complete = null;
            }
            else
            {
                Graphic_.AnimationState.Complete = (Track) =>
                {
                    Finished?.Invoke();
                    Graphic_.AnimationState.Complete = null;
                };
            }
        }
    }
}