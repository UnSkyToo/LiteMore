using LiteFramework.Core.Base;

namespace LiteMore.Combat.Buff
{
    public static class BuffManager
    {
        private static readonly ListEx<BaseBuff> BuffList_ = new ListEx<BaseBuff>();
        private static float DeltaTime_ = 0;

        public static bool Startup()
        {
            BuffList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Buff in BuffList_)
            {
                Buff.Dispose();
            }
            BuffList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            DeltaTime_ = DeltaTime;
            BuffList_.Foreach((Entity) =>
            {
                Entity.Tick(DeltaTime_);

                if (!Entity.IsAlive)
                {
                    Entity.Dispose();
                    BuffList_.Remove(Entity);
                }
            });
        }

        public static BaseBuff AddBuff(BaseBuff Buff)
        {
            BuffList_.Add(Buff);
            Buff.TryAttach();
            return Buff;
        }

        public static void RemoveBuff(BaseBuff Buff)
        {
            if (Buff == null)
            {
                return;
            }

            Buff.IsAlive = false;
        }
    }
}