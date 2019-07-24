using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Label
{
    public static class LabelManager
    {
        private static readonly List<BaseLabel> LabelList_ = new List<BaseLabel>();

        public static bool Startup()
        {
            LabelList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in LabelList_)
            {
                Entity.Destroy();
            }
            LabelList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = LabelList_.Count - 1; Index >= 0; --Index)
            {
                LabelList_[Index].Tick(DeltaTime);

                if (!LabelList_[Index].IsAlive)
                {
                    LabelList_[Index].Destroy();
                    LabelList_.RemoveAt(Index);
                }
            }
        }

        public static NumberLabel AddNumberLabel(Vector2 Position, NumberLabelType Type, int Value)
        {
            return null;

            var Obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Num"));
            Obj.transform.SetParent(Configure.LabelRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new NumberLabel(Obj.transform, Type, Value);
            Entity.Create();
            LabelList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }
    }
}