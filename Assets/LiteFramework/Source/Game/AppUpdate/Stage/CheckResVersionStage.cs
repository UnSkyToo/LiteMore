namespace Lite.Framework.AppUpdate.Stage
{
    public class CheckResVersionStage : IStage
    {
        public CheckResVersionStage(Updater Updater)
            : base(Updater, StageID.CheckResVersion)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.CheckResVersionBegin);
            Updater_.SetProgressValue(0);
            Updater_.ClearError();

            ResPatchInfoList PatchInfoList = Updater_.GetResPatchInfoList();

            if (PatchInfoList == null)
            {
                PatchInfoList = UpdateTool.GetResPatchInfoList();
                if (PatchInfoList == null)
                {
                    StageCode_ = StageCode.Failed;
                    Updater_.SetError(ErrorCode.GetResPatchInfoListFailed, UpdateTool.ServerVersionInfoPath_);
                    return;
                }

                Updater_.SetResPatchInfoList(PatchInfoList);
            }

            ResPatchInfo PatchInfo = PatchInfoList.GetPatchWithVersion(Updater_.GetCurrentVersionInfo());
            if (PatchInfo != null)
            {
                Updater_.SetCurrentResPatchInfo(PatchInfo);
            }
            else
            {
                SetNextStageID(StageID.Final);
            }

            StageCode_ = StageCode.Succeeded;
            Updater_.SetProgressID(StageProgressID.CheckResVersionEnd);
            Updater_.SetProgressValue(1.0f);
        }
    }
}