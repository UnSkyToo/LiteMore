using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public static class NpcManager
    {
        private static Transform NpcRoot_;
        private static GameObject ModelPrefab_;
        private static readonly List<NpcBase> NpcList_ = new List<NpcBase>();

        public static bool Startup()
        {
            NpcRoot_ = GameObject.Find("Npc").transform;

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

        public static NpcBase AddNpc(Vector2 Position)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(NpcRoot_, false);
            Obj.transform.localPosition = Position;
            
            var Entity = new NpcBase(Obj.transform);
            Entity.Create();
            NpcList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static int GetCount()
        {
            return NpcList_.Count;
        }

        public static List<NpcBase> GetNpcList()
        {
            return NpcList_;
        }

        public static NpcBase FindNpc(uint ID)
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

        public static NpcBase GetRandomNpc()
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