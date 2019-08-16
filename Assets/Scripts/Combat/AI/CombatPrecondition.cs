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
            if (NextSkill != null)
            {
                return true;
            }

            NextSkill = Input.Attacker.GetSkillList()[0];
            Input.Attacker.SetNextSkill(NextSkill);
            Input.Distance = NextSkill.Radius;
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
            var Dist = Vector2.Distance(Input.Attacker.Position, Input.Attacker.TargetNpc.Position);
            if (Dist > 0 && Dist <= Input.Distance)
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
            var Dist = Vector2.Distance(Input.Attacker.Position, Input.Attacker.TargetNpc.Position);
            if (Dist > Input.Distance)
            {
                return true;
            }

            return false;
        }
    }
}