using System;
using System.IO;
using System.Threading;

namespace Lite.Framework.AppUpdate.Stage
{
    public class CopyResInfoToDocStage : IStage
    {
        private long CurSize_;
        private long MaxSize_;
        private bool IsDone_;
        private string ErrorInfo_;
        private string UnZipInfo_;
        private object ProgressLockObject_;
        private Thread UnZipThread_ = null;

        public CopyResInfoToDocStage(Updater Updater)
            : base(Updater, StageID.CopyResInfoToDoc)
        {
        }

        protected override void OnEnter()
        {
            Updater_.SetProgressID(StageProgressID.CopyResInfoToDocBegin);
            Updater_.SetProgressValue(0);
            Updater_.ClearError();

            ResPatchInfo PatchInfo = Updater_.GetCurrentResPatchInfo();

            if (PatchInfo == null)
            {
                StageCode_ = StageCode.Failed;
                Updater_.SetError(ErrorCode.GetResPatchInfoListFailed, UpdateTool.ServerVersionInfoPath_);
                return;
            }

            CurSize_ = 0;
            MaxSize_ = 1;
            IsDone_ = false;
            ErrorInfo_ = string.Empty;
            UnZipInfo_ = string.Empty;
            ProgressLockObject_ = new object();

            string PatchDataPath = UpdateTool.DataPath_ + "\\" + Path.GetFileName(PatchInfo.PatchUrl_);
            UnZipThread_ = new Thread(UnZipFunc);
            UnZipThread_.Start(PatchDataPath);
        }

        protected override void OnExit()
        {
            if (StageCode_ == StageCode.Succeeded)
            {
                ResPatchInfo PatchInfo = Updater_.GetCurrentResPatchInfo();

                if (PatchInfo == null)
                {
                    return;
                }

                UpdateTool.UpdateResVersionInfo(PatchInfo.To_);
                Updater_.SetCurrentVersionInfo(PatchInfo.To_);
                Updater_.SetCurrentResPatchInfo(null);
            }
        }

        protected override void OnUpdate()
        {
            lock (ProgressLockObject_)
            {
                float Value = (float) CurSize_ / (float) MaxSize_;
                Updater_.SetProgressValue(Value);
                Updater_.SetContentInfo(UnZipInfo_);

                if (IsDone_)
                {
                    UnZipThread_.Join();
                    UnZipThread_ = null;
                    Updater_.SetProgressID(StageProgressID.CopyResInfoToDocEnd);
                    Updater_.SetProgressValue(1.0f);
                    
                    if (string.IsNullOrEmpty(ErrorInfo_))
                    {
                        StageCode_ = StageCode.Succeeded;
                    }
                    else
                    {
                        StageCode_ = StageCode.Failed;
                        Updater_.SetError(ErrorCode.CopyResInfoToDocFailed, ErrorInfo_);
                    }
                }
            }
        }

        private void UnZipFunc(object zipFilePath)
        {
            try
            {
                ZipUtil.UnZipFileEx((string) zipFilePath, string.Empty, string.Empty, true, (FileName, CurSize, MaxSize) =>
                {
                    lock (ProgressLockObject_)
                    {
                        UnZipInfo_ = FileName;
                        CurSize_ = CurSize;
                        MaxSize_ = MaxSize;
                    }
                });

                File.Delete((string) zipFilePath);

                lock (ProgressLockObject_)
                {
                    ErrorInfo_ = string.Empty;
                    UnZipInfo_ = string.Empty;
                    IsDone_ = true;
                }
            }
            catch (Exception Ex)
            {
                lock (ProgressLockObject_)
                {
                    ErrorInfo_ = Ex.Message;
                    UnZipInfo_ = string.Empty;
                    IsDone_ = true;
                }
            }
        }
    }
}