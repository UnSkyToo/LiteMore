using System.Collections.Generic;
using LiteFramework.Helper;
using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Skill.Executor
{
    /// <summary>
    /// 普攻
    /// </summary>
    public class SkillExecutor_3001 : BaseExecutor
    {
        public override bool Execute(Dictionary<string, object> Args)
        {
            var Master = Args["Master"] as BaseNpc;
            if (Master == null)
            {
                return false;
            }

            if (Master.IsValidTarget())
            {
                var Damage = CombatManager.Calculator.Calc(Master.CalcFinalAttr(NpcAttrIndex.Damage), 1);
                Master.TargetNpc.OnHitDamage(Master, "Atk", Damage);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 嘲讽
    /// </summary>
    public class SkillExecutor_3002 : BaseExecutor
    {
        public override bool Execute(Dictionary<string, object> Args)
        {
            var Master = Args["Master"] as BaseNpc;
            if (Master == null)
            {
                return false;
            }

            if (Master.IsValidTarget())
            {
                var TargetList = LockingHelper.Find(Master, new LockingRule(LockTeamType.Enemy, LockNpcType.InDistance), Args["Radius"]);
                if (TargetList.Count > 0)
                {
                    foreach (var Target in TargetList)
                    {
                        Target.ForceTarget(Master);
                    }

                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 分身
    /// </summary>
    public class SkillExecutor_3003 : BaseExecutor
    {
        public override bool Execute(Dictionary<string, object> Args)
        {
            var Master = Args["Master"] as BaseNpc;
            if (Master == null)
            {
                return false;
            }

            var Slave = NpcManager.AddNpc($"{Master.Name}_clone", Master.Position + UnityHelper.RandVec2(100), Master.Team, new NpcAttribute(Master.Attr));
            Slave.Scale = Master.Scale;
            Slave.SetDirection(Master.Direction);
            return true;
        }
    }
}