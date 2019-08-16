using System.Collections.Generic;
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
}