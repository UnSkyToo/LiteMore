namespace Lite.Framework.AppUpdate.Stage
{
    public class DownloadResInfoStage : IStage
    {
        public DownloadResInfoStage(Updater Updater)
            : base(Updater, StageID.DownloadResInfo)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.DownloadResInfoBegin);
            Updater_.SetProgressValue(0);
            Updater_.ClearError();
            StageCode_ = StageCode.Continue;

            DownloadTask Task = Downloader.Start(Updater_.GetResVersionInfo().ResUrl_, UpdateTool.ServerVersionInfoPath_, string.Empty, true);
            Task.Progress += OnProgressCallback;
            Task.Completed += OnCompletedCallback;
        }

        private void OnProgressCallback(string FileName, string StateName, long CurSize, long MaxSize, long Speed)
        {
            Updater_.SetProgressValue((float)CurSize / (float)MaxSize);
        }

        private void OnCompletedCallback(string FileName, string ErrorInfo, bool Successed)
        {
            Updater_.SetProgressID(StageProgressID.DownloadResInfoEnd);
            Updater_.SetProgressValue(1.0f);

            if (Successed)
            {
                StageCode_ = StageCode.Succeeded;
            }
            else
            {
                StageCode_ = StageCode.Failed;
                Updater_.SetError(ErrorCode.DownloadResInfoFailed, ErrorInfo);
            }
        }
    }
}
