namespace Lite.Framework.AppUpdate.Stage
{
    public class IStage
    {
        protected Updater Updater_ = null;
        protected StageID StageID_;
        protected StageID NextStageID_;
        protected StageCode StageCode_;

        public IStage(Updater Updater, StageID StageID)
        {
            StageID_ = StageID;
            Updater_ = Updater;
            NextStageID_ = StageID.Error;
        }

        public StageID GetStageID()
        {
            return StageID_;
        }

        public StageID GetNextStageID()
        {
            return NextStageID_;
        }

        public void SetNextStageID(StageID NextStageID)
        {
            NextStageID_ = NextStageID;
        }

        public void Enter()
        {
            StageCode_ = StageCode.Continue;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }

        public void Update()
        {
            OnUpdate();
        }

        public StageCode CheckStage()
        {
            return OnCheckStage();
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual StageCode OnCheckStage()
        {
            return StageCode_;
        }
    }
}
