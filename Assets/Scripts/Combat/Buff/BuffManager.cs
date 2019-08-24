using LiteFramework.Core.Base;

namespace LiteMore.Combat.Buff
{
    public static class BuffManager
    {
        private static readonly ListEx<BaseBuff> BuffList_ = new ListEx<BaseBuff>();

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
            foreach (var Buff in BuffList_)
            {
                Buff.Tick(DeltaTime);

                if (!Buff.IsAlive)
                {
                    Buff.Dispose();
                    BuffList_.Remove(Buff);
                }
            }
            BuffList_.Flush();
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