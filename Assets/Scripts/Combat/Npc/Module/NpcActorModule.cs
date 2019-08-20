using LiteMore.Combat.Fsm;
using UnityEngine;

namespace LiteMore.Combat.Npc.Module
{
    public class NpcActorModule : BaseNpcModule
    {
        public NpcDirection Direction { get; private set; }

        private readonly BaseFsm Fsm_;
        private readonly NpcAnimation Animation_;

        public NpcActorModule(BaseNpc Master)
            : base(Master)
        {
            Animation_ = new NpcAnimation(Master.GetComponent<Animator>());
            Fsm_ = new BaseFsm(Master);

            Direction = NpcDirection.Right;
        }

        public override void Tick(float DeltaTime)
        {
            Animation_.Tick(DeltaTime);
            Fsm_.Tick(DeltaTime);
        }

        public void OnCombatEvent(CombatEvent Event)
        {
            Fsm_.OnCombatEvent(Event);
        }

        private void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            Fsm_.OnMsgCode(Animation, MsgCode);
        }

        public uint RegisterMsg(string Animation, float Percent, CombatMsgCode MsgCode)
        {
            return Animation_.RegisterMsg(Animation, Percent, MsgCode, OnMsgCode);
        }

        public void UnRegisterMsg(uint ID)
        {
            Animation_.UnRegisterMsg(ID);
        }

        public bool HasAnimation(string AnimName)
        {
            return Animation_.HasAnimation(AnimName);
        }

        public void PlayAnimation(string AnimName, bool IsLoop)
        {
            Animation_.Play(AnimName, IsLoop);
        }

        public bool AnimationIsEnd()
        {
            return Animation_.IsEnd();
        }

        public bool IsFsmState(FsmStateName StateName)
        {
            return Fsm_.GetStateName() == StateName;
        }

        public void ChangeToIdleState()
        {
            Fsm_.ChangeToIdleState();
        }

        public void SetDirection(NpcDirection Dir)
        {
            if (Direction == Dir)
            {
                return;
            }

            Direction = Dir;
            switch (Direction)
            {
                case NpcDirection.Left:
                    Master.GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case NpcDirection.Right:
                    Master.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                default:
                    break;
            }
        }

        public void FaceToPosition(Vector2 TargetPosition)
        {
            SetDirection(CombatHelper.CalcDirection(Master.Position, TargetPosition));
        }
    }
}