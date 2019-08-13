using System.IO;

namespace Lite.Framework.AppUpdate.Stage
{
    public class DownloadResStage : IStage
    {
        public DownloadResStage(Updater Updater)
            : base(Updater, StageID.DownloadRes)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.DownloadResBegin);
            Updater_.SetProgressValue(0);
            Updater_.ClearError();
            StageCode_ = StageCode.Continue;

            ResPatchInfo PatchInfo = Updater_.GetCurrentResPatchInfo();

            if (PatchInfo == null)
            {
                StageCode_ = StageCode.Failed;
                Updater_.SetError(ErrorCode.GetResPatchInfoListFailed, UpdateTool.ServerVersionInfoPath_);
                return;
            }

            Updater_.SetContentInfo(string.Format("{0}->{1}", PatchInfo.From_, PatchInfo.To_));

            string PatchDataPath = Path.Combine(UpdateTool.DataPath_, Path.GetFileName(PatchInfo.PatchUrl_));
            DownloadTask Task = Downloader.Start(PatchInfo.PatchUrl_, PatchDataPath, PatchInfo.MD5_, true);
            Task.Progress += OnProgressCallback;
            Task.Completed += OnCompletedCallback;
        }
        private void OnProgressCallback(string FileName, string StateName, long CurSize, long MaxSize, long Speed)
        {
            Updater_.SetProgressValue((float)CurSize / (float)MaxSize);
        }

        private void OnCompletedCallback(string FileName, string ErrorInfo, bool Successed)
        {
            Updater_.SetProgressID(StageProgressID.DownloadResEnd);
            Updater_.SetProgressValue(1.0f);

            if (Successed)
            {
                StageCode_ = StageCode.Succeeded;
            }
            else
            {
                StageCode_ = StageCode.Failed;
                Updater_.SetError(ErrorCode.DownloadResFailed, ErrorInfo);
            }
        }
    }
}