using System.Collections.Generic;
using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Skill.Executor
{
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
}