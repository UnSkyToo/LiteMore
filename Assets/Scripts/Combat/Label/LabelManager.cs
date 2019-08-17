using System.Collections.Generic;
using LiteFramework.Game.Asset;
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
                Entity.Dispose();
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
                    LabelList_[Index].Dispose();
                    LabelList_.RemoveAt(Index);
                }
            }
        }

        public static NumberLabel AddNumberLabel(Vector2 Position, NumberLabelType Type, float Value)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/num.prefab");
            Obj.transform.SetParent(Configure.LabelRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new NumberLabel(Obj.transform, Type, $"{Value:0.0}");
            LabelList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static StringLabel AddStringLabel(Vector2 Position, string Value, Color TextColor, int FontSize = 20)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/num.prefab");
            Obj.transform.SetParent(Configure.LabelRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new StringLabel(Obj.transform, Value, TextColor, FontSize);
            LabelList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }
    }
}