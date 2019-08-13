namespace Lite.Framework.AppUpdate.Stage
{
    public class GetResVersionStage : IStage
    {
        public GetResVersionStage(Updater Updater)
            : base(Updater, StageID.GetResVersion)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.GetResVersionBegin);
            Updater_.SetProgressValue(0);
            Updater_.ClearError();

            ResVersionInfo VersionInfo = UpdateTool.GetResVersionInfo();

            if (VersionInfo == null)
            {
                StageCode_ = StageCode.Failed;
                Updater_.SetError(ErrorCode.GetResVersionInfoFailed, UpdateTool.VersionInfoPath_);
            }
            else
            {
                Updater_.SetResVersionInfo(VersionInfo);
                Updater_.SetCurrentVersionInfo(VersionInfo.Version_);
                StageCode_ = StageCode.Succeeded;
            }

            Updater_.SetProgressID(StageProgressID.GetResVersionEnd);
            Updater_.SetProgressValue(1.0f);
        }
    }
}
