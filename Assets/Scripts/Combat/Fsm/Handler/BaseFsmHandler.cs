namespace LiteMore.Combat.Fsm.Handler
{
    public class BaseFsmHandler
    {
        protected readonly BaseFsm Fsm_;

        public BaseFsmHandler(BaseFsm Fsm)
        {
            Fsm_ = Fsm;
        }

        public virtual void OnTick(float DeltaTime)
        {
        }

        public virtual bool OnCombatEvent(CombatEvent Event)
        {
            return false;
        }

        public virtual bool OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            return false;
        }
    }
}