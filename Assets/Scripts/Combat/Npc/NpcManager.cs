using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Skill;
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
        // Range,      // 攻击范围
        // Radius,     // 可攻击半径
        public static NpcAttribute GenerateInitAttr(float Speed, float MaxHp, float MaxMp, float Damage, float Gem, float Range, float Radius)
        {
            return new NpcAttribute(new float[]
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
            });
        }

        public static NpcAttribute GenerateCoreNpcAttr()
        {
            return new NpcAttribute(new float[]
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
            });
        }

        public static BaseNpc AddNpc(string Name, Vector2 Position, CombatTeam Team, NpcAttribute InitAttr)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/npc/r2/r2.prefab");
            Obj.transform.SetParent(Configure.NpcRoot, false);
            Obj.transform.localPosition = Position;

            var Entity = new AINpc(Name, Obj.transform, Team, InitAttr);
            NpcList_[(int)Team].Add(Entity);

            Entity.Position = Position;

            var NormalAtk = SkillManager.AddNpcSkill(SkillLibrary.Get(3001), Entity);
            NormalAtk.Radius = Entity.CalcFinalAttr(NpcAttrIndex.Radius);
            Entity.AddSkill(NormalAtk);

            return Entity;
        }

        public static CoreNpc AddCoreNpc(string Name, Vector2 Position, NpcAttribute InitAttr)
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

        public static int GetCount()
        {
            return NpcList_[(int)CombatTeam.A].Count + NpcList_[(int)CombatTeam.B].Count;
        }
    }
}