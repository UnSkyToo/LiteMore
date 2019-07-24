using System.Collections.Generic;
using LiteMore.Combat.AI.BehaviorTree;

namespace LiteMore.Combat.AI
{
    public class Behavior_TryFindTarget : BehaviorPrecondition
    {
        public Behavior_TryFindTarget()
            : base()
        {
        }

        public override bool ExternalCondition(BehaviorInputData Input)
        {
            /*Role attacker = RoleManager.GetInstance().FindRole(Input.Attacker);

            if (attacker.GetRoleType() == Role.RoleType.Monster)
            {
                input.TargetId = RoleManager.GetInstance().MainPlayer.Id;
            }
            else
            {
                List<Role> monster = RoleManager.GetInstance().MonsterList;

                foreach (Role role in monster)
                {
                    input.TargetId = role.Id;
                    break;
                }
            }*/

            return true;
        }
    }
}