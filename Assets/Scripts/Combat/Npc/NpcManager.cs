using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteFramework.Game.Asset;
using LiteMore.Player;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public static class NpcManager
    {
        public const int TeamCount = 2;

        private static readonly List<BaseNpc>[] NpcList_ = new List<BaseNpc>[TeamCount]
        {
            new List<BaseNpc>(),
            new List<BaseNpc>()
        };

        private static CoreNpc CoreNpc_;

        public static bool Startup()
        {
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
                    Entity.Dispose();
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
                        NpcList_[Team][Index].Dispose();
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
        // AtkRange,   // 攻击范围
        // HitRange,   // 受击范围
        public static float[] GenerateInitAttr(float Speed, float MaxHp, float HpAdd, float MaxMp, float MpAdd, float Damage, float Gem, float AtkRange, float HitRange)
        {
            return new float[]
            {
                Speed,
                MaxHp,
                MaxHp,
                HpAdd,
                MaxMp,
                MaxMp,
                MpAdd,
                Damage,
                Gem,
                AtkRange,
                HitRange,
            };
        }

        public static float[] GenerateCoreNpcAttr()
        {
            return new float[]
            {
                0,
                PlayerManager.Player.Hp,
                PlayerManager.Player.MaxHp,
                PlayerManager.Player.AddHp,
                PlayerManager.Player.Mp,
                PlayerManager.Player.MaxMp,
                PlayerManager.Player.AddMp,
                0,
                0,
                0,
                50,
            };
        }

        public static AINpc AddNpc(string Name, Vector2 Position, CombatTeam Team, float[] InitAttr)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/npc/r2/r2.prefab");
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new AINpc(Name, Obj.transform, Team, InitAttr);
            NpcList_[(int)Team].Add(Entity);

            Entity.Position = Position;
            Entity.Skill.AddNpcSkill(1001).Radius = CombatHelper.GetNpcAtkRange(Entity);

            EventManager.Send(new NpcAddEvent(Entity));
            return Entity;
        }

        public static CoreNpc AddCoreNpc(string Name, Vector2 Position, float[] InitAttr)
        {
            if (CoreNpc_ != null)
            {
                return CoreNpc_;
            }

            var Obj = AssetManager.CreatePrefabSync("prefabs/npc/core/core.prefab");
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            CoreNpc_ = new CoreNpc(Name, Obj.transform, InitAttr);
            NpcList_[(int)CombatTeam.A].Add(CoreNpc_);
            CoreNpc_.Position = Position;
            CoreNpc_.IsStatic = true;

            EventManager.Send(new NpcAddEvent(CoreNpc_));
            return CoreNpc_;
        }

        public static List<BaseNpc> GetNpcList(CombatTeam Team)
        {
            var Result = new List<BaseNpc>();

            foreach (var Npc in NpcList_[(int)Team])
            {
                if (Npc.IsValid())
                {
                    Result.Add(Npc);
                }
            }

            return Result;
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

        public static int GetCount()
        {
            return NpcList_[(int)CombatTeam.A].Count + NpcList_[(int)CombatTeam.B].Count;
        }
    }
}