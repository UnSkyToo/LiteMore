using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Bullet;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    /// <summary>
    /// 近战普攻
    /// </summary>
    public class SkillExecutor_1001 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Master = Args.Master;
            if (Master == null)
            {
                return false;
            }

            var TargetList = FilterHelper.Find(Args.LockRule, new FilterArgs()
            {
                Master = Master,
                Team = Master.Team,
                Position = Master.Position,
                Radius = Args.Skill.Radius,
                Rotation = Master.Rotation,
                Shape = Args.Skill.Shape
            });

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

    /// <summary>
    /// 弓箭普攻
    /// </summary>
    public class SkillExecutor_1002 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Master = Args.Master;
            if (Master == null)
            {
                return false;
            }

            var TargetList = FilterHelper.Find(Args.LockRule, new FilterArgs()
            {
                Master = Master,
                Team = Master.Team,
                Position = Master.Position,
                Radius = Args.Skill.Radius,
                Rotation = Master.Rotation,
                Shape = Args.Skill.Shape
            });

            if (TargetList.Count > 0)
            {
                var Damage = CombatManager.Calculator.Calc(Master.CalcFinalAttr(NpcAttrIndex.Damage), 1);
                BulletManager.AddArrowBullet(
                    new ArrowBulletDescriptor(
                        new BaseBulletDescriptor(Args.Skill.Name, Args.Master.Position, Args.Master.Team, Damage),
                        TargetList[0].Position, 1000, new Color(0.5f, 0.3f, 0.4f)));
            }

            return false;
        }
    }
}