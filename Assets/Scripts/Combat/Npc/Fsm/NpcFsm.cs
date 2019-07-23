using System.Collections.Generic;

namespace LiteMore.Combat.Npc.Fsm
{
    public class NpcFsm
    {
        public BaseNpc Master { get; }

        private readonly Dictionary<NpcFsmStateName, NpcFsmStateBase> StateList_;
        private NpcFsmStateBase CurrentState_;

        public NpcFsm(BaseNpc Master)
        {
            this.Master = Master;
            this.StateList_ = new Dictionary<NpcFsmStateName, NpcFsmStateBase>
            {
                {NpcFsmStateName.Idle, new NpcFsmStateIdle(this)},
                {NpcFsmStateName.Walk, new NpcFsmStateWalk(this)},
                {NpcFsmStateName.Attack, new NpcFsmStateAttack(this)},
                {NpcFsmStateName.Die, new NpcFsmStateDie(this)},
                {NpcFsmStateName.Back, new NpcFsmStateBack(this)},
            };

        }

        public void Tick(float DeltaTime)
        {
            CurrentState_?.OnTick(DeltaTime);
        }

        public void OnEvent(NpcEvent Event)
        {
            CurrentState_?.OnEvent(Event);
        }

        public NpcFsmStateName GetStateName()
        {
            return CurrentState_?.StateName ?? NpcFsmStateName.Idle;
        }

        public void ChangeToState(NpcFsmStateName StateName, NpcEvent Event)
        {
            CurrentState_?.OnExit();
            CurrentState_ = StateList_[StateName];
            CurrentState_?.OnEnter(Event);
        }

        public void ChangeToIdleState()
        {
            ChangeToState(NpcFsmStateName.Idle, null);
        }
    }
}