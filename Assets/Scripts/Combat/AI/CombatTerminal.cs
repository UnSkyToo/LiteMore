using LiteMore.Combat.AI.BehaviorTree;

namespace LiteMore.Combat.AI
{
    public class CombatTerminal_MoveToTarget : BehaviorTerminalNode
    {
        protected override void OnEnter(BehaviorInputData Input)
        {
            if (!Input.Attacker.IsFsmState(FsmStateName.Walk))
            {
                if (Input.Attacker.IsValidTarget())
                {
                    Input.Attacker.MoveTo(Input.Attacker.TargetNpc.Position);
                }
            }
        }

        protected override BehaviorRunningState OnExecute(BehaviorInputData Input)
        {
            if (Input.Attacker.IsFsmState(FsmStateName.Walk))
            {
                return BehaviorRunningState.Running;
            }

            return BehaviorRunningState.Finish;
        }
    }

    public class CombatTerminal_UseSkill : BehaviorTerminalNode
    {
        protected override void OnEnter(BehaviorInputData Input)
        {
            Input.Attacker.UseNextSkill();
        }

        protected override BehaviorRunningState OnExecute(BehaviorInputData Input)
        {
            if (Input.Attacker.IsFsmState(FsmStateName.Skill))
            {
                return BehaviorRunningState.Running;
            }

            return BehaviorRunningState.Finish;
        }
    }
}