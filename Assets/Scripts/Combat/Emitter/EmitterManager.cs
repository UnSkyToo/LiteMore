using System.Collections.Generic;

namespace LiteMore.Combat.Emitter
{
    public static class EmitterManager
    {
        private static readonly List<BaseEmitter> EmitterList_ = new List<BaseEmitter>();

        public static bool Startup()
        {
            EmitterList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in EmitterList_)
            {
                Entity.Dispose();
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
                    EmitterList_[Index].Dispose();
                    EmitterList_.RemoveAt(Index);
                }
            }
        }

        public static BaseEmitter AddEmitter(BaseEmitter Emitter)
        {
            EmitterList_.Add(Emitter);
            Emitter.CreateDebugLine();
            return Emitter;
        }

        public static void RemoveEmitter(BaseEmitter Emitter)
        {
            if (Emitter == null)
            {
                return;
            }

            Emitter.IsAlive = false;
            Emitter.Dispose();
            EmitterList_.Remove(Emitter);
        }

        public static int GetCount()
        {
            return EmitterList_.Count;
        }
    }
}