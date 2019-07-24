using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Fsm.Handler;
using LiteMore.Combat.Fsm.State;

namespace LiteMore.Combat.Fsm
{
    public class BaseFsm
    {
        public BaseNpc Master { get; }

        private readonly Dictionary<FsmStateName, BaseState> StateList_;
        private readonly List<BaseFsmHandler> HandlerList_;
        private BaseState CurrentState_;

        public BaseFsm(BaseNpc Master)
        {
            this.Master = Master;
            this.StateList_ = new Dictionary<FsmStateName, BaseState>
            {
                {FsmStateName.Idle, new IdleState(this)},
                {FsmStateName.Walk, new WalkState(this)},
                {FsmStateName.Attack, new AttackState(this)},
                {FsmStateName.Die, new DieState(this)},
                {FsmStateName.Back, new BackState(this)},
            };

            this.HandlerList_ = new List<BaseFsmHandler>();
        }

        public void Tick(float DeltaTime)
        {
            foreach (var Handler in HandlerList_)
            {
                Handler.OnTick(DeltaTime);
            }

            CurrentState_?.OnTick(DeltaTime);
        }

        public void OnCombatEvent(CombatEvent Event)
        {
            foreach (var Handler in HandlerList_)
            {
                if (Handler.OnCombatEvent(Event))
                {
                    return;
                }
            }

            CurrentState_?.OnCombatEvent(Event);
        }

        public void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            foreach (var Handler in HandlerList_)
            {
                if (Handler.OnMsgCode(Animation, MsgCode))
                {
                    return;
                }
            }
            
            CurrentState_?.OnMsgCode(Animation, MsgCode);
        }

        public FsmStateName GetStateName()
        {
            return CurrentState_?.StateName ?? FsmStateName.Idle;
        }

        public void ChangeToState(FsmStateName StateName, CombatEvent Event)
        {
            CurrentState_?.OnExit();
            CurrentState_ = StateList_[StateName];
            CurrentState_?.OnEnter(Event);
        }

        public void ChangeToIdleState()
        {
            ChangeToState(FsmStateName.Idle, null);
        }

        public void RegisterHandler(BaseFsmHandler Handler)
        {
            if (HandlerList_.Contains(Handler))
            {
                return;
            }

            HandlerList_.Add(Handler);
        }

        public void UnRegisterHandler(BaseFsmHandler Handler)
        {
            HandlerList_.Remove(Handler);
        }
    }
}