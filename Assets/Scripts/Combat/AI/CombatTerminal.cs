using LiteMore.Combat.AI.BehaviorTree;
using UnityEngine;

namespace LiteMore.Combat.AI
{
    public class CombatTerminal_MoveToTarget : BehaviorTerminalNode
    {
        protected override void OnEnter(BehaviorInputData Input)
        {
            if (!Input.Attacker.Actor.IsFsmState(FsmStateName.Walk))
            {
                if (Input.Attacker.Action.IsValidTarget())
                {
                    Input.Attacker.Action.MoveTo(Input.Attacker.Action.TargetNpc.Position);
                }
            }
        }

        protected override BehaviorRunningState OnExecute(BehaviorInputData Input)
        {
            if (Input.Attacker.Actor.IsFsmState(FsmStateName.Walk) && Input.Attacker.Action.IsValidTarget())
            {
                if (!CombatHelper.InAttackRange(Input.Attacker, Input.Attacker.Action.TargetNpc))
                {
                    Input.Attacker.Action.MoveTo(Input.Attacker.Action.TargetNpc.Position);
                }

                return BehaviorRunningState.Running;
            }

            return BehaviorRunningState.Finish;
        }
    }

    public class CombatTerminal_UseSkill : BehaviorTerminalNode
    {
        protected override void OnEnter(BehaviorInputData Input)
        {
            var Args = Input.Attacker.Skill.CreateSkillArgs(Input.SkillID);
            Args.Position = Input.Attacker.Action.TargetNpc.Position;
            Args.Direction = (Input.Attacker.Action.TargetNpc.Position - Input.Attacker.Position).normalized;
            Input.Attacker.Skill.UseSkill(Args);
        }

        protected override BehaviorRunningState OnExecute(BehaviorInputData Input)
        {
            if (Input.Attacker.Actor.IsFsmState(FsmStateName.Skill))
            {
                return BehaviorRunningState.Running;
            }

            return BehaviorRunningState.Finish;
        }
    }
}