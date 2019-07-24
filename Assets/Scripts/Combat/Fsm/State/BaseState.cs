namespace LiteMore.Combat.Fsm.State
{
    public abstract class BaseState
    {
        public FsmStateName StateName { get; }
        public BaseFsm Fsm { get; }

        protected BaseState(FsmStateName StateName, BaseFsm Fsm)
        {
            this.StateName = StateName;
            this.Fsm = Fsm;
        }

        public virtual void OnEnter(CombatEvent Event) { }
        public virtual void OnExit() { }
        public virtual void OnTick(float DeltaTime) { }
        public virtual void OnCombatEvent(CombatEvent Event) { }
        public virtual void OnMsgCode(string Animation, CombatMsgCode MsgCode) { }
    }
}