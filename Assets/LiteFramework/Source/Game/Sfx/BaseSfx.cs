using System;
using LiteFramework.Game.Base;
using UnityEngine;

namespace LiteFramework.Game.Sfx
{
    public abstract class BaseSfx : GameEntity
    {
        protected BaseSfx(string Name, Transform Trans)
            : base(Name, Trans, false, false)
        {
        }

        public abstract bool IsEnd();

        public abstract void Play(string AnimationName, bool IsLoop = false, Action Callback = null);

        public void Stop()
        {
            IsAlive = false;
        }
    }
}