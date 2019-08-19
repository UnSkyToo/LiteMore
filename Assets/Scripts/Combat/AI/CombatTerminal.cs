using LiteMore.Combat.AI.BehaviorTree;
using UnityEngine;

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
            if (Input.Attacker.IsFsmState(FsmStateName.Walk) && Input.Attacker.IsValidTarget())
            {
                var Dist = Vector2.Distance(Input.Attacker.TargetPos, Input.Attacker.TargetNpc.Position);
                if (Dist > Input.Attacker.CalcFinalAttr(NpcAttrIndex.Range))
                {
                    Input.Attacker.MoveTo(Input.Attacker.TargetNpc.Position);
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
            Input.Attacker.UseSkill(Input.Attacker.CreateSkillArgs(Input.SkillID));
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