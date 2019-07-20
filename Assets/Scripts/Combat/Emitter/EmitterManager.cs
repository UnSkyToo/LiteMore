using System.Collections.Generic;

namespace LiteMore.Combat.Emitter
{
    public static class EmitterManager
    {
        private static readonly List<EmitterBase> EmitterList_ = new List<EmitterBase>();

        public static bool Startup()
        {
            EmitterList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in EmitterList_)
            {
                Entity.Destroy();
            }

            EmitterList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = EmitterList_.Count - 1; Index >= 0; --Index)
            {
                EmitterList_[Index].Tick(DeltaTime);

                if (!EmitterList_[Index].IsAlive)
                {
                    EmitterList_[Index].Destroy();
                    EmitterList_.RemoveAt(Index);
                }
            }
        }

        public static EmitterBase AddEmitter(EmitterBase Emitter)
        {
            EmitterList_.Add(Emitter);
            Emitter.Create();
            return Emitter;
        }

        public static int GetCount()
        {
            return EmitterList_.Count;
        }
    }
}