using LiteMore.Combat.AI.Filter;

namespace LiteMore.Combat.Skill.Executor
{
    /// <summary>
    /// 普攻
    /// </summary>
    public class SkillExecutor_1001 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Master = Args.Skill.Master;
            if (Master == null)
            {
                return false;
            }

            var TargetList = FilterHelper.Find(Args.Skill, Args.LockRule);
            foreach (var Target in TargetList)
            {
                if (!Target.IsValid())
                {
                    continue;
                }

                var Damage = CombatManager.Calculator.Calc(Master.CalcFinalAttr(NpcAttrIndex.Damage), 1);
                Target.OnHitDamage(Master, Args.Skill.Name, Damage);
                Target.TryToPlayHitSfx(Args.HitSfx);
            }

            return true;
        }
    }
}