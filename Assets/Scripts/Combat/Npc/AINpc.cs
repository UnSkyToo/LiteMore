using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class AINpc : BaseNpc
    {
        public AINpc(string Name, Transform Trans, CombatTeam Team, float[] InitAttr)
            : base(Name, Trans, Team, InitAttr)
        {
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (IsFsmState(FsmStateName.Idle))
            {
                if (!IsValidTarget())
                {
                    TargetNpc = FindTarget();
                }

                MoveToTarget();
            }
            else if (IsFsmState(FsmStateName.Walk))
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
                        EventManager.Send(new NpcAttackEvent(this));
                    }
                }
            }
            else if (IsFsmState(FsmStateName.Attack))
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