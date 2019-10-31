using System;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Group
{
    public static class GroupManager
    {
        private static readonly ListEx<GroupEntity> GroupList_ = new ListEx<GroupEntity>();

        public static bool Startup()
        {
            GroupList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in GroupList_)
            {
                Entity.Dispose();
            }
            GroupList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            GroupList_.Foreach((Entity) =>
            {
                if (Entity.IsEnd)
                {
                    Entity.Dispose();
                    GroupList_.Remove(Entity);
                }
            });
        }

        public static GroupEntity CreateSequenceGroup(Action Callback)
        {
            var Entity = new GroupEntity(false, Callback);
            GroupList_.Add(Entity);
            return Entity;
        }

        public static GroupEntity CreateParallelGroup(Action Callback)
        {
            var Entity = new GroupEntity(true, Callback);
            GroupList_.Add(Entity);
            return Entity;
        }
    }
}