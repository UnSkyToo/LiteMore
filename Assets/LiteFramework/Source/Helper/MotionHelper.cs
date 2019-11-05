using System;
using System.Collections.Generic;
using LiteFramework.Core.Motion;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class MotionHelper
    {
        public static MotionContainer Sequence(bool IsRepeat = false)
        {
            return new MotionContainer(IsRepeat ? MotionContainer.MotionContainerType.RepeatSequence : MotionContainer.MotionContainerType.Sequence);
        }

        public static MotionContainer Parallel()
        {
            return new MotionContainer(MotionContainer.MotionContainerType.Parallel);
        }
    }

    public class MotionContainer
    {
        public enum MotionContainerType
        {
            Sequence,
            RepeatSequence,
            Parallel,
        }

        public MotionContainer Parent { get; set; }
        
        private readonly MotionContainerType ContainerType_;
        private readonly List<BaseMotion> MotionList_;

        public MotionContainer(MotionContainerType ContainerType)
        {
            ContainerType_ = ContainerType;
            MotionList_ = new List<BaseMotion>();
        }

        public MotionContainer BeginSequence(bool IsRepeat = false)
        {
            var Container = MotionHelper.Sequence(IsRepeat);
            Container.Parent = this;
            return Container;
        }

        public MotionContainer EndSequence()
        {
            if (Parent == null)
            {
                return null;
            }

            Parent.Motion(Flush());
            return Parent;
        }

        public MotionContainer BeginParallel()
        {
            var Container = MotionHelper.Parallel();
            Container.Parent = this;
            return Container;
        }

        public MotionContainer EndParallel()
        {
            if (Parent == null)
            {
                return null;
            }

            Parent.Motion(Flush());
            return Parent;
        }

        public MotionContainer Motion(BaseMotion Motion)
        {
            MotionList_.Add(Motion);
            return this;
        }

        public MotionContainer Wait(float Time)
        {
            MotionList_.Add(new WaitTimeMotion(Time));
            return this;
        }

        public MotionContainer Move(float Time, Vector3 Position, bool IsRelative)
        {
            MotionList_.Add(new MoveMotion(Time, Position, IsRelative));
            return this;
        }

        public MotionContainer Scale(float Time, Vector3 Scale, bool IsRelative)
        {
            MotionList_.Add(new ScaleMotion(Time, Scale, IsRelative));
            return this;
        }

        public MotionContainer Rotate(float Time, Quaternion Rotate)
        {
            MotionList_.Add(new RotateMotion(Time, Rotate));
            return this;
        }

        public MotionContainer Fade(float Time, float BeginAlpha, float EndAlpha)
        {
            MotionList_.Add(new FadeMotion(Time, BeginAlpha, EndAlpha));
            return this;
        }

        public MotionContainer FadeIn(float Time)
        {
            MotionList_.Add(new FadeInMotion(Time));
            return this;
        }

        public MotionContainer FadeOut(float Time)
        {
            MotionList_.Add(new FadeOutMotion(Time));
            return this;
        }

        public MotionContainer Callback(Action Callback)
        {
            MotionList_.Add(new CallbackMotion(Callback));
            return this;
        }

        public MotionContainer Callback<T>(Action<T> Callback, T Param)
        {
            MotionList_.Add(new CallbackMotion<T>(Callback, Param));
            return this;
        }

        public MotionContainer Active(bool Value)
        {
            MotionList_.Add(new ActiveMotion(Value));
            return this;
        }

        public MotionContainer Destroy()
        {
            MotionList_.Add(new DestroyMotion());
            return this;
        }

        public MotionContainer UpdateTransform(Vector2 Position)
        {
            MotionList_.Add(new UpdateTransformMotion(Position));
            return this;
        }

        public MotionContainer UpdateTransform(Vector2 Position, Vector2 Scale)
        {
            MotionList_.Add(new UpdateTransformMotion(Position, Scale));
            return this;
        }

        public MotionContainer UpdateTransform(Vector2 Position, Vector2 Scale, Quaternion Rotation)
        {
            MotionList_.Add(new UpdateTransformMotion(Position, Scale, Rotation));
            return this;
        }

        public BaseMotion Flush()
        {
            switch (ContainerType_)
            {
                case MotionContainerType.Sequence:
                    return new SequenceMotion(MotionList_.ToArray());
                case MotionContainerType.RepeatSequence:
                    return new RepeatSequenceMotion(MotionList_.ToArray());
                case MotionContainerType.Parallel:
                    return new ParallelMotion(MotionList_.ToArray());
            }

            return null;
        }
    }
}