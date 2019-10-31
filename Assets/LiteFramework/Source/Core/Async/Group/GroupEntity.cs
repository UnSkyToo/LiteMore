using System;
using System.Collections.Generic;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Group
{
    public class GroupEntity : BaseObject, IDisposable
    {
        public bool IsEnd { get; private set; }

        private readonly List<GroupItem> ItemList_;
        private readonly bool IsParallel_;
        private int DoneCount_;
        private Action Callback_;

        public GroupEntity(bool IsParallel, Action Callback)
            : base()
        {
            IsEnd = false;
            IsParallel_ = IsParallel;
            DoneCount_ = 0;
            Callback_ = Callback;
            ItemList_ = new List<GroupItem>();
        }

        public void Dispose()
        {
            Callback_ = null;
            ItemList_.Clear();
        }

        public void Execute()
        {
            IsEnd = false;
            DoneCount_ = 0;

            if (ItemList_.Count == 0)
            {
                ItemDone();
                return;
            }

            if (IsParallel_)
            {
                foreach (var Item in ItemList_)
                {
                    Item.Execute();
                }
            }
            else
            {
                ItemList_[DoneCount_].Execute();
            }
        }

        public GroupItem CreateItem(Action<GroupItem> Func)
        {
            var Item = new GroupItem(this, Func);
            ItemList_.Add(Item);
            return Item;
        }

        public GroupParamItem<T> CreateParamItem<T>(Action<GroupItem, T> Func, T Param)
        {
            var Item = new GroupParamItem<T>(this, Func, Param);
            ItemList_.Add(Item);
            return Item;
        }

        public GroupWaitTime CreateWaitItem(float WaitTime)
        {
            var Item = new GroupWaitTime(this, WaitTime);
            ItemList_.Add(Item);
            return Item;
        }

        public void ItemDone()
        {
            DoneCount_++;

            if (DoneCount_ >= ItemList_.Count)
            {
                IsEnd = true;
                Callback_?.Invoke();
            }

            if (!IsParallel_)
            {
                ItemList_[DoneCount_].Execute();
            }
        }
    }
}