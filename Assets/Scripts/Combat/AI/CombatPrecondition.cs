using LiteMore.Combat.AI.BehaviorTree;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.AI
{
    public class CombatPrecondition_TryFindSkill : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (!Input.Attacker.Skill.CanUseSkill())
            {
                return false;
            }

            var NextSkill = Input.Attacker.Skill.GetSkill(Input.SkillID);
            if (NextSkill != null && NextSkill.CanUse())
            {
                return true;
            }

            var SkillList = Input.Attacker.Skill.GetSkillList();
            foreach (var Skill in SkillList)
            {
                if (Skill.Type != SkillType.Passive && Skill.CanUse())
                {
                    NextSkill = Skill;
                    break;
                }
            }

            if (NextSkill == null)
            {
                return false;
            }

            Input.SkillID = NextSkill.SkillID;
            //Input.Distance = NextSkill.Radius;
            return true;
        }
    }

    public class CombatPrecondition_TryFindSkillTarget : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (Input.Attacker.Action.IsValidTarget())
            {
                return true;
            }

            Input.Attacker.Action.TargetNpc = FilterHelper.FindNearest(Input.Attacker);
            return Input.Attacker.Action.TargetNpc != null;
        }
    }

    public class CombatPrecondition_IsNearTarget : BehaviorPrecondition
    {
        public override bool ExternalCondition(BehaviorInputData Input)
        {
            if (CombatHelper.InAttackRange(Input.Attacker, Input.Attacker.Action.TargetNpc))
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
            if (!CombatHelper.InAttackRange(Input.Attacker, Input.Attacker.Action.TargetNpc))
            {
                return true;
            }

            return false;
        }
    }
}