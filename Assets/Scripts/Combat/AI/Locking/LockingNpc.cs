using System;
using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using UnityEngine;

namespace LiteMore.Combat.AI.Locking
{
    public static class LockingNpc
    {
        private static readonly Dictionary<LockNpcType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>> FuncList_ = new Dictionary<LockNpcType, Func<List<BaseNpc>, BaseSkill, List<BaseNpc>>>
        {
            {LockNpcType.All, Find_All},
            {LockNpcType.Nearest, Find_Nearest},
            {LockNpcType.CurHpMinimum, Find_CurHpMinimum},
            {LockNpcType.CurHpMaximum, Find_CurHpMaximum},
            {LockNpcType.MaxHpMinimum, Find_MaxHpMinimum},
            {LockNpcType.MaxHpMaximum, Find_MaxHpMaximum},
            {LockNpcType.HpPercentMinimum, Find_HpPercentMinimum},
            {LockNpcType.HpPercentMaximum, Find_HpPercentMaximum},
            {LockNpcType.DamageMinimum, Find_DamageMinimum},
            {LockNpcType.DamageMaximum, Find_DamageMaximum},
            {LockNpcType.Random, Find_Random},
        };

        public static List<BaseNpc> Find(List<BaseNpc> List, BaseSkill Skill, LockNpcType Type)
        {
            return FuncList_[Type].Invoke(List, Skill);
        }

        private static List<BaseNpc> Find_All(List<BaseNpc> List, BaseSkill Skill)
        {
            return List;
        }

        private static List<BaseNpc> Find_Nearest(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Dist = Vector2.Distance(Skill.Master.Position, Npc.Position);
                if (Dist < Value)
                {
                    Value = Dist;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> {Target};
        }

        private static List<BaseNpc> Find_CurHpMinimum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Hp = Npc.CalcFinalAttr(NpcAttrIndex.Hp);
                if (Hp < Value)
                {
                    Value = Hp;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_CurHpMaximum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MinValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Hp = Npc.CalcFinalAttr(NpcAttrIndex.Hp);
                if (Hp > Value)
                {
                    Value = Hp;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_MaxHpMinimum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Hp = Npc.CalcFinalAttr(NpcAttrIndex.MaxHp);
                if (Hp < Value)
                {
                    Value = Hp;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_MaxHpMaximum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MinValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Hp = Npc.CalcFinalAttr(NpcAttrIndex.MaxHp);
                if (Hp > Value)
                {
                    Value = Hp;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_HpPercentMinimum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Percent = Npc.CalcFinalAttr(NpcAttrIndex.Hp) / Npc.CalcFinalAttr(NpcAttrIndex.MaxHp);
                if (Percent < Value)
                {
                    Value = Percent;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_HpPercentMaximum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MinValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Percent = Npc.CalcFinalAttr(NpcAttrIndex.Hp) / Npc.CalcFinalAttr(NpcAttrIndex.MaxHp);
                if (Percent > Value)
                {
                    Value = Percent;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_DamageMinimum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MaxValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Damage = Npc.CalcFinalAttr(NpcAttrIndex.Damage);
                if (Damage < Value)
                {
                    Value = Damage;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_DamageMaximum(List<BaseNpc> List, BaseSkill Skill)
        {
            var Value = float.MinValue;
            BaseNpc Target = null;

            foreach (var Npc in List)
            {
                var Damage = Npc.CalcFinalAttr(NpcAttrIndex.Damage);
                if (Damage > Value)
                {
                    Value = Damage;
                    Target = Npc;
                }
            }

            return Target == null ? new List<BaseNpc>() : new List<BaseNpc> { Target };
        }

        private static List<BaseNpc> Find_Random(List<BaseNpc> List, BaseSkill Skill)
        {
            if (List.Count == 0)
            {
                return List;
            }

            var Index = UnityEngine.Random.Range(0, List.Count);
            return new List<BaseNpc> { List[Index] };
        }
    }
}