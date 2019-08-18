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

            var TargetList = Args["TargetList"] as List<BaseNpc>;
            if (TargetList == null || TargetList.Count == 0)
            {
                return false;
            }

            foreach (var Target in TargetList)
            {
                if (!Target.IsValid())
                {
                    continue;
                }

                var Damage = CombatManager.Calculator.Calc(Master.CalcFinalAttr(NpcAttrIndex.Damage), 1);
                Target.OnHitDamage(Master, "Atk", Damage);
                Target.TryToPlayHitSfx((string)Args["HitSfx"]);
            }

            return true;
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

            var TargetList = Args["TargetList"] as List<BaseNpc>;
            if (TargetList == null || TargetList.Count == 0)
            {
                return false;
            }

            foreach (var Target in TargetList)
            {
                if (!Target.IsValid())
                {
                    continue;
                }

                Target.ForceTarget(Master);
            }

            return true;
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