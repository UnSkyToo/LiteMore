using System;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Group
{
    public class GroupItem : BaseObject, IDisposable
    {
        public bool IsEnd { get; private set; }
        protected readonly GroupEntity Master_;
        protected Action<GroupItem> Func_;

        public GroupItem(GroupEntity Master, Action<GroupItem> Func)
            : base()
        {
            Master_ = Master;
            Func_ = Func;

            IsEnd = false;
        }

        public virtual void Dispose()
        {
            Func_ = null;
        }

        public virtual void Execute()
        {
            Func_?.Invoke(this);
        }

        public void Done()
        {
            Master_?.ItemDone();
        }
    }

    public class GroupParamItem<T> : GroupItem
    {
        private readonly Action<GroupItem, T> NewFunc_;
        private readonly T Param_;

        public GroupParamItem(GroupEntity Master, Action<GroupItem, T> Func, T Param)
            : base(Master, null)
        {
            NewFunc_ = Func;
            Param_ = Param;
        }

        public override void Execute()
        {
            NewFunc_?.Invoke(this, Param_);
        }
    }

    public class GroupWaitTime : GroupItem
    {
        private readonly float WaitTime_;
        private TimerEntity Timer_;

        public GroupWaitTime(GroupEntity Master, float WaitTime)
            : base(Master, null)
        {
            WaitTime_ = WaitTime;
        }

        public override void Dispose()
        {
            TimerManager.StopTimer(Timer_);
            Timer_ = null;

            base.Dispose();
        }

        public override void Execute()
        {
            Timer_ = TimerManager.AddTimer(WaitTime_, Done, 1);
        }
    }
}