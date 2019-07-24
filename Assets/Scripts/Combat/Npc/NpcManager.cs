using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public static class NpcManager
    {
        public const int TeamCount = 2;

        private static GameObject ModelPrefab_;

        private static readonly List<BaseNpc>[] NpcList_ = new List<BaseNpc>[TeamCount]
        {
            new List<BaseNpc>(),
            new List<BaseNpc>()
        };

        private static CoreNpc CoreNpc_;

        public static bool Startup()
        {
            ModelPrefab_ = Resources.Load<GameObject>("Prefabs/Npc/R2/R2");
            if (ModelPrefab_ == null)
            {
                Debug.LogError("NpcManager : null model prefab");
                return false;
            }

            for (var Team = 0; Team < TeamCount; ++Team)
            {
                NpcList_[Team].Clear();
            }

            CoreNpc_ = null;

            return true;
        }

        public static void Shutdown()
        {
            CoreNpc_ = null;

            for (var Team = 0; Team < TeamCount; ++Team)
            {
                foreach (var Entity in NpcList_[Team])
                {
                    Entity.Destroy();
                }
                NpcList_[Team].Clear();
            }
        }

        public static void Tick(float DeltaTime)
        {
            for (var Team = 0; Team < TeamCount; ++Team)
            {
                for (var Index = NpcList_[Team].Count - 1; Index >= 0; --Index)
                {
                    NpcList_[Team][Index].Tick(DeltaTime);

                    if (!NpcList_[Team][Index].IsAlive)
                    {
                        NpcList_[Team][Index].Destroy();
                        NpcList_[Team].RemoveAt(Index);
                    }
                }
            }
        }

        // copy form NpcAttrIndex enum in NpcDefined.cs
        // Speed = 0,  // 移动速度
        // Hp,         // 当前生命值
        // MaxHp,      // 最大生命值
        // HpAdd,      // 生命恢复
        // Mp,         // 当前魔法值
        // MaxMp,      // 最大魔法值
        // MpAdd,      // 魔法恢复
        // Damage,     // 伤害
        // Gem,        // 死亡奖励宝石
        // Range,      // 攻击范围
        // Radius,     // 可攻击半径
        public static float[] GenerateInitAttr(float Speed, float MaxHp, float MaxMp, float Damage, float Gem, float Range, float Radius)
        {
            return new float[]
            {
                Speed,
                MaxHp,
                MaxHp,
                0,
                MaxMp,
                MaxMp,
                0,
                Damage,
                Gem,
                Range,
                Radius,
            };
        }

        public static float[] GenerateCoreNpcAttr()
        {
            return new float[]
            {
                0,
                PlayerManager.Hp,
                PlayerManager.MaxHp,
                PlayerManager.HpAdd,
                PlayerManager.Mp,
                PlayerManager.MaxMp,
                PlayerManager.MpAdd,
                0,
                0,
                0,
                50,
            };
        }

        public static BaseNpc AddNpc(Vector2 Position, CombatTeam Team, float[] InitAttr, bool ForceFirst = false)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new AINpc(Obj.transform, Team, InitAttr);
            Entity.Create();
            if (ForceFirst)
            {
                NpcList_[(int)Team].Insert(0, Entity);
            }
            else
            {
                NpcList_[(int)Team].Add(Entity);
            }

            Entity.Position = Position;

            return Entity;
        }

        public static CoreNpc AddCoreNpc(Vector2 Position, float[] InitAttr)
        {
            if (CoreNpc_ != null)
            {
                return CoreNpc_;
            }

            var Obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Npc/Core/Core"));
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            CoreNpc_ = new CoreNpc(Obj.transform, InitAttr);
            CoreNpc_.Create();
            NpcList_[(int)CombatTeam.A].Add(CoreNpc_);
            CoreNpc_.Position = Position;
            CoreNpc_.IsStatic = true;

            return CoreNpc_;
        }

        public static List<BaseNpc> GetNpcList(CombatTeam Team)
        {
            return NpcList_[(int)Team];
        }

        public static BaseNpc FindNpc(uint ID)
        {
            if (ID == 0)
            {
                return null;
            }

            var Npc = FindNpc(CombatTeam.A, ID);
            if (Npc != null)
            {
                return Npc;
            }

            return FindNpc(CombatTeam.B, ID);
        }

        public static BaseNpc FindNpc(CombatTeam Team, uint ID)
        {
            if (ID == 0)
            {
                return null;
            }

            foreach (var Entity in NpcList_[(int)Team])
            {
                if (Entity.ID == ID)
                {
                    return Entity;
                }
            }

            return null;
        }

        public static BaseNpc GetRandomNpc(CombatTeam Team)
        {
            if (GetCount() == 0)
            {
                return null;
            }

            for (var Index = 0; Index < NpcList_[(int)Team].Count; ++Index)
            {
                if (NpcList_[(int)Team][Index].CanLocked())
                {
                    return NpcList_[(int)Team][Index];
                }
            }

            return null;
        }

        public static void OnCombatEvent(CombatEvent Event)
        {
            var Entity = FindNpc(Event.ID);
            Entity?.OnCombatEvent(Event);
        }

        public static int GetCount()
        {
            return NpcList_[(int)CombatTeam.A].Count + NpcList_[(int)CombatTeam.B].Count;
        }
    }
}