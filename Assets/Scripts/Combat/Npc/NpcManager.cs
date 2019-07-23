using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public static class NpcManager
    {
        private static GameObject ModelPrefab_;
        private static readonly List<BaseNpc> NpcList_ = new List<BaseNpc>();

        public static bool Startup()
        {
            ModelPrefab_ = Resources.Load<GameObject>("Prefabs/R2");
            if (ModelPrefab_ == null)
            {
                Debug.LogError("NpcManager : null model prefab");
                return false;
            }

            NpcList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in NpcList_)
            {
                Entity.Destroy();
            }
            NpcList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = NpcList_.Count - 1; Index >= 0; --Index)
            {
                NpcList_[Index].Tick(DeltaTime);

                if (!NpcList_[Index].IsAlive)
                {
                    NpcList_[Index].Destroy();
                    NpcList_.RemoveAt(Index);
                }
            }
        }

        // copy form NpcAttrIndex enum in NpcAttribute.cs
        // Speed = 0,      // 移动速度
        // Hp,             // 当前生命值
        // MaxHp,          // 最大生命值
        // Damage,         // 伤害
        // Gem,            // 死亡奖励宝石
        public static float[] GenerateInitAttr(float Speed, float MaxHp, float Damage, float Gem)
        {
            return new float[]
            {
                Speed,
                MaxHp,
                MaxHp,
                Damage,
                Gem
            };
        }

        public static BaseNpc AddNpc(Vector2 Position, float[] InitAttr)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new BaseNpc(Obj.transform, InitAttr);
            Entity.Create();
            NpcList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static int GetCount()
        {
            return NpcList_.Count;
        }

        public static List<BaseNpc> GetNpcList()
        {
            return NpcList_;
        }

        public static BaseNpc FindNpc(uint ID)
        {
            foreach (var Entity in NpcList_)
            {
                if (Entity.ID == ID)
                {
                    return Entity;
                }
            }

            return null;
        }

        public static BaseNpc GetRandomNpc()
        {
            if (GetCount() == 0)
            {
                return null;
            }

            for (var Index = 0; Index < NpcList_.Count; ++Index)
            {
                if (NpcList_[Index].ForecastHp > 0)
                {
                    return NpcList_[Index];
                }
            }

            return null;
        }

        public static void OnNpcEvent(NpcEvent Event)
        {
            var Entity = FindNpc(Event.ID);
            Entity?.OnEvent(Event);
        }
    }
}