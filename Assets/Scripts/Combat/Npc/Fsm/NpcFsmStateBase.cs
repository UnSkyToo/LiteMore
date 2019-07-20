namespace LiteMore.Combat.Npc.Fsm
{
    public enum NpcFsmStateName
    {
        Idle,
        Walk,
        Attack,
        Die
    }

    public abstract class NpcFsmStateBase
    {
        public NpcFsmStateName StateName { get; }
        public NpcFsm Fsm { get; }

        protected NpcFsmStateBase(NpcFsmStateName StateName, NpcFsm Fsm)
        {
            this.StateName = StateName;
            this.Fsm = Fsm;
        }

        public virtual void OnEnter(NpcEvent Event) { }
        public virtual void OnExit() { }
        public virtual void OnTick(float DeltaTime) { }
        public virtual void OnEvent(NpcEvent Event) { }
    }
}