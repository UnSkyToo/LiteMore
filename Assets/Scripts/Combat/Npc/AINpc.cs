using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class AINpc : BaseNpc
    {
        public AINpc(Transform Trans, CombatTeam Team, float[] InitAttr)
            : base(Trans, Team, InitAttr)
        {
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsIdleState())
            {
                if (!IsValidTarget())
                {
                    TargetNpc = FindTarget();
                }

                MoveToTarget();
            }
            else if (IsWalkState())
            {
                if (!IsValidTarget())
                {
                    Fsm_.ChangeToIdleState();
                }
                else
                {
                    var Distance = Vector2.Distance(Position, TargetNpc.Position);
                    if (Distance <= CalcFinalAttr(NpcAttrIndex.Range) + TargetNpc.CalcFinalAttr(NpcAttrIndex.Radius))
                    {
                        Fsm_.ChangeToState(FsmStateName.Attack, new NpcAttackEvent(TargetNpc.ID));
                    }
                }
            }
            else if (IsAttackState())
            {
                if (!IsValidTarget())
                {
                    Fsm_.ChangeToIdleState();
                }
                else
                {
                    var Distance = Vector2.Distance(Position, TargetNpc.Position);
                    if (Distance > CalcFinalAttr(NpcAttrIndex.Range) + TargetNpc.CalcFinalAttr(NpcAttrIndex.Radius))
                    {
                        Fsm_.ChangeToIdleState();
                    }
                }
            }
        }

        private void MoveToTarget()
        {
            if (IsValidTarget())
            {
                MoveTo(TargetNpc.Position);
            }
        }

        private BaseNpc FindTarget()
        {
            return NpcManager.GetRandomNpc(Team.Opposite());
        }
    }
}