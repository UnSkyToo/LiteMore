using LiteMore.Combat.AI.BehaviorTree;
using LiteMore.Combat.AI.Locking;
using UnityEngine;

namespace LiteMore.Combat.AI
{
    public class CombatPrecondition_TryFindSkill : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (!Input.Attacker.CanUseSkill())
            {
                return false;
            }

            var NextSkill = Input.Attacker.GetNextSkill();
            if (NextSkill != null && NextSkill.CanUse())
            {
                return true;
            }

            var SkillList = Input.Attacker.GetSkillList();
            for (var Index = SkillList.Count - 1; Index >= 0; --Index)
            {
                if (SkillList[Index].CanUse())
                {
                    NextSkill = SkillList[Index];
                    break;
                }
            }

            if (NextSkill == null)
            {
                return false;
            }

            Input.Attacker.SetNextSkill(NextSkill);
            //Input.Distance = NextSkill.Radius;
            return true;
        }
    }

    public class CombatPrecondition_TryFindSkillTarget : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (Input.Attacker.IsValidTarget())
            {
                return true;
            }

            Input.Attacker.TargetNpc = LockingHelper.FindNearest(Input.Attacker);
            return Input.Attacker.TargetNpc != null;
        }
    }

    public class CombatPrecondition_IsNearTarget : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (CombatHelper.IsAttackRange(Input.Attacker, Input.Attacker.TargetNpc))
            {
                return true;
            }

            return false;
        }
    }

    public class CombatPrecondition_IsFarTarget : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (!CombatHelper.IsAttackRange(Input.Attacker, Input.Attacker.TargetNpc))
            {
                return true;
            }

            return false;
        }
    }
}