namespace Lite.Framework.AppUpdate.Stage
{
    public class FinalStage : IStage
    {
        public FinalStage(Updater Updater)
            : base(Updater, StageID.Final)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.Done);
            Updater_.SetProgressValue(1.0f);
        }

        protected override StageCode OnCheckStage()
        {
            return StageCode.Succeeded;
        }
    }
}