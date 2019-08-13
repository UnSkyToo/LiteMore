namespace Lite.Framework.AppUpdate.Stage
{
    public class ErrorStage : IStage
    {
        public ErrorStage(Updater Updater)
            : base(Updater, StageID.Error)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.Error);
            Updater_.SetProgressValue(0);
        }
    }
}