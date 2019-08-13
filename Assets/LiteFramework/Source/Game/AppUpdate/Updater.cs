namespace Lite.Framework.AppUpdate
{
    public class Updater
    {
        private StageProgressID ProgressID_;
        private float ProgressValue_;
        private ResVersionInfo ResVersionInfo_;
        private VersionInfo CurrentVersionInfo_;
        private ResPatchInfoList ResPatchInfoList_;
        private ResPatchInfo CurrentPatchInfo_;
        private ErrorCode ErrorCode_;
        private string ErrorInfo_;
        private string ContentInfo_;

        private Pipline Pipline_;
        
        public Updater()
        {
            Start();
        }

        public void Start()
        {
            ProgressID_ = StageProgressID.Idle;
            ProgressValue_ = 0;
            ResVersionInfo_ = null;
            CurrentVersionInfo_ = null;
            ResPatchInfoList_ = null;
            CurrentPatchInfo_ = null;
            ErrorCode_ = ErrorCode.Ok;
            ErrorInfo_ = string.Empty;
            ContentInfo_ = string.Empty;

            Pipline_ = new Pipline();
            Pipline_.InitStage(this);

            Downloader.StopAllTask();
        }

        public void Stop()
        {
            Pipline_.ClearStage();
            Downloader.StopAllTask();
        }

        public void Update(float DeltaTime)
        {
            Pipline_.Update();
            Downloader.Update(DeltaTime);
        }

        public bool IsDone()
        {
            return ProgressID_ == StageProgressID.Done;
        }

        public void SetProgressID(StageProgressID ProgressID)
        {
            ProgressID_ = ProgressID;
        }

        public StageProgressID GetProgressID()
        {
            return ProgressID_;
        }

        public void SetProgressValue(float Value)
        {
            ProgressValue_ = Value;
        }

        public float GetProgressValue()
        {
            return ProgressValue_;
        }

        public void SetCurrentVersionInfo(VersionInfo Info)
        {
            CurrentVersionInfo_ = Info;
        }

        public VersionInfo GetCurrentVersionInfo()
        {
            return CurrentVersionInfo_;
        }

        public void SetResVersionInfo(ResVersionInfo VersionInfo)
        {
            ResVersionInfo_ = VersionInfo;
        }

        public ResVersionInfo GetResVersionInfo()
        {
            return ResVersionInfo_;
        }

        public void SetResPatchInfoList(ResPatchInfoList PatchInfoList)
        {
            ResPatchInfoList_ = PatchInfoList;
        }

        public ResPatchInfoList GetResPatchInfoList()
        {
            return ResPatchInfoList_;
        }

        public void SetCurrentResPatchInfo(ResPatchInfo PatchInfo)
        {
            CurrentPatchInfo_ = PatchInfo;
        }

        public ResPatchInfo GetCurrentResPatchInfo()
        {
            return CurrentPatchInfo_;
        }
        
        public string GetErrorInfo()
        {
            return ErrorInfo_;
        }
        
        public ErrorCode GetErrorCode()
        {
            return ErrorCode_;
        }

        public void ClearError()
        {
            ErrorInfo_ = string.Empty;
            ErrorCode_ = ErrorCode.Ok;
        }

        public void SetError(ErrorCode Code, string Msg = "")
        {
            ErrorCode_ = Code;
            ErrorInfo_ = Msg;
        }

        public void SetContentInfo(string Info)
        {
            ContentInfo_ = Info;
        }

        public string GetContentInfo()
        {
            return ContentInfo_;
        }
    }
}
