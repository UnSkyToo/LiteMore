using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using LiteFramework.Core.Base;

namespace LiteFramework.AppUpdate
{
    public enum DownloadState
    {
        Wait, // 等待开始下载
        OpenWriteStream, // 打开写入文件流
        RequestFileSize, // 请求文件大小
        RequestDownload, // 请求下载文件
        OpenReadStream, // 打开下载文件流
        Downloading, // 下载数据中
        Completed, // 下载完成
        Error, // 下载错误
    }

    public class DownloadTask
    {
        private const string TempFileExt_ = "tmp";

        public delegate void ProgressCallback(string FileName, string StageName, long CurSize, long MaxSize, long Speed);

        public delegate void CompletedCallback(string FileName, string ErrorInfo, bool Succeeded);

        public event ProgressCallback Progress;
        public event CompletedCallback Completed;

        private uint ID_;
        private int BufferSize_;
        private int DownloadCount_;
        private long DownloadSize_;
        private long FileSize_;
        private long RangeSize_;
        private long CheckSpeedSize_;
        private long DownloadSpeed_;
        private string ErrorInfo_ = string.Empty;
        private string RequestUrl_;
        private string WritePath_;
        private string TempWritePath_;
        private string FileName_;
        private string MD5_;
        private float Time_;
        private bool Overwrite_;
        private byte[] Buffer_;
        private HttpWebRequest Request_ = null;
        private HttpWebResponse Response_ = null;
        private Stream ReadStream_ = null;
        private Stream WriteStream_ = null;
        private DownloadState State_;

        private static object CallbackLock_ = new object();

        public uint ID
        {
            get { return ID_; }
        }

        public int DownloadCount
        {
            get { return DownloadCount_; }
        }

        public string FileName
        {
            get { return FileName_; }
        }

        public DownloadTask(string RequestUrl, string WritePath, string MD5, bool Overwrite, int BufferSize)
        {
            ID_ = IDGenerator.Get();
            RequestUrl_ = RequestUrl;
            WritePath_ = WritePath;
            TempWritePath_ = string.Format("{0}.{1}", WritePath_, TempFileExt_);
            BufferSize_ = BufferSize;
            FileName_ = Path.GetFileName(RequestUrl);
            MD5_ = MD5.ToUpper();
            Overwrite_ = Overwrite;

            Reset();
        }

        public void Reset()
        {
            CloseStream();
            CloseRequest();

            Buffer_ = new byte[BufferSize_];
            Request_ = null;
            Response_ = null;
            ReadStream_ = null;
            ErrorInfo_ = string.Empty;
            DownloadSize_ = 0;
            FileSize_ = 0;
            RangeSize_ = 0;
            CheckSpeedSize_ = 0;
            DownloadSpeed_ = 0;
            Time_ = 0;
            ChangeState(DownloadState.Wait);
        }

        public void Start()
        {
            if (!Overwrite_ && File.Exists(WritePath_))
            {
                Abort("file exist : " + WritePath_);
                return;
            }

            DownloadCount_++;

            if (OpenWriteStream())
            {
                RequestFileSize();
            }
        }

        public void Stop()
        {
            Abort("stop");
        }

        public void Update(float DeltaTime)
        {
            Time_ += DeltaTime;

            if (Time_ >= 1.0f)
            {
                Time_ -= 1.0f;

                DownloadSpeed_ = DownloadSize_ - CheckSpeedSize_;
                CheckSpeedSize_ = DownloadSize_;
            }
        }

        public bool IsEnd()
        {
            return State_ == DownloadState.Completed || State_ == DownloadState.Error;
        }

        public bool IsCompleted()
        {
            return State_ == DownloadState.Completed;
        }

        public void InvokeProgressCallback()
        {
            lock (CallbackLock_)
            {
                if (Progress != null)
                {
                    long MaxSize = FileSize_ > 0 ? FileSize_ : 1;
                    long CurSize = DownloadSize_ > FileSize_ ? FileSize_ : DownloadSize_;

                    Progress(FileName_, State_.ToString(), CurSize, MaxSize, DownloadSpeed_);
                }
            }
        }

        public void InvokeCompletedCallback()
        {
            lock (CallbackLock_)
            {
                if (Completed != null)
                {
                    Completed(FileName_, ErrorInfo_, State_ == DownloadState.Completed);
                }
            }
        }

        private void ChangeState(DownloadState State)
        {
            State_ = State;
        }

        private void Abort(string Msg)
        {
            ChangeState(DownloadState.Error);
            ErrorInfo_ = Msg;
            CloseStream();
            CloseRequest();
        }

        private void Done()
        {
            ChangeState(DownloadState.Completed);
            CloseStream();
            CloseRequest();
        }

        private void CloseRequest()
        {
            try
            {
                if (Request_ != null)
                {
                    Request_.Abort();
                    Request_ = null;
                }

                if (Response_ != null)
                {
                    Response_.Close();
                    Response_ = null;
                }
            }
            catch(Exception Ex)
            {
                Abort("Exception CloseRequest : " + Ex.Message);
            }
        }

        private void CloseStream()
        {
            try
            {
                if (ReadStream_ != null)
                {
                    ReadStream_.Close();
                    ReadStream_.Dispose();
                    ReadStream_ = null;
                }

                if (WriteStream_ != null)
                {
                    WriteStream_.Close();
                    WriteStream_.Dispose();
                    WriteStream_ = null;
                }
            }
            catch(Exception Ex)
            {
                Abort("Exception CloseStream : " + Ex.Message);
            }
        }

        private bool OpenWriteStream()
        {
            try
            {
                ChangeState(DownloadState.OpenWriteStream);

                if (File.Exists(TempWritePath_))
                {
                    WriteStream_ = File.Open(TempWritePath_, FileMode.Open, FileAccess.ReadWrite);
                    WriteStream_.Seek(0, SeekOrigin.End);
                }
                else
                {
                    WriteStream_ = File.Create(TempWritePath_);
                }

                RangeSize_ = WriteStream_.Length;
                return true;
            }
            catch(Exception Ex)
            {
                Abort("open file error : " + TempWritePath_ + "\r\n" + Ex.Message);
                return false;
            }
        }

        private void RequestFileSize()
        {
            try
            {
                ChangeState(DownloadState.RequestFileSize);
                HttpWebRequest Request = WebRequest.Create(RequestUrl_) as HttpWebRequest;

                if (Request != null)
                {
                    Request.KeepAlive = true;
                    Request.Method = WebRequestMethods.File.DownloadFile;
                    Request.Timeout = 5000;
                    Request_ = Request;

                    Request.BeginGetResponse(OnGetSizeResponse, null);
                }
                else
                {
                    Abort("request url error : " + RequestUrl_);
                }
            }
            catch(Exception Ex)
            {
                Abort("Exception RequestFileSize : " + Ex.Message);
            }
        }

        private void OnGetSizeResponse(IAsyncResult Result)
        {
            try
            {
                HttpWebResponse Response = Request_.EndGetResponse(Result) as HttpWebResponse;
                FileSize_ = Response.ContentLength;
                CloseRequest();

                if (RangeSize_ >= FileSize_)
                {
                    DownloadSize_ = FileSize_;
                    CheckDownloadCompleted();
                }
                else
                {
                    DownloadSize_ = RangeSize_;
                    RequestDownload();
                }
            }
            catch (Exception Ex)
            {
                Abort("Exception OnGetSizeResponse : " + Ex.Message);
            }
        }

        private void RequestDownload()
        {
            try
            {
                ChangeState(DownloadState.RequestDownload);
                HttpWebRequest Request = WebRequest.Create(RequestUrl_) as HttpWebRequest;

                if (Request != null)
                {
                    Request.KeepAlive = true;
                    Request.Method = WebRequestMethods.File.DownloadFile;
                    Request.Timeout = 5000;
                    Request.AddRange((int)RangeSize_);
                    Request_ = Request;

                    Request.BeginGetResponse(OnGetResponse, null);
                }
                else
                {
                    Abort("request url error : " + RequestUrl_);
                }
            }
            catch (Exception Ex)
            {
                Abort("Exception RequestDownload : " + Ex.Message);
            }
        }

        private void OnGetResponse(IAsyncResult Result)
        {
            try
            {
                ChangeState(DownloadState.OpenWriteStream);
                Response_ = Request_.EndGetResponse(Result) as HttpWebResponse;
                ReadStream_ = Response_.GetResponseStream();
                ReadStream_.BeginRead(Buffer_, 0, BufferSize_, OnRead, null);
            }
            catch (Exception Ex)
            {
                Abort("Exception OnGetResponse : " + Ex.Message);
            }
        }

        private void OnRead(IAsyncResult Result)
        {
            try
            {
                ChangeState(DownloadState.Downloading);
                int ReadSize = ReadStream_.EndRead(Result);

                if (ReadSize > 0)
                {
                    WriteStream_.Write(Buffer_, 0, ReadSize);
                    WriteStream_.Flush();
                    DownloadSize_ += ReadSize;
                    ReadStream_.BeginRead(Buffer_, 0, BufferSize_, OnRead, null);
                }
                else
                {
                    CheckDownloadCompleted();
                }
            }
            catch (Exception Ex)
            {
                Abort("Exception OnRead : " + Ex.Message);
            }
        }

        private void CheckDownloadCompleted()
        {
            if (DownloadSize_ == FileSize_)
            {
                WriteStream_.Close();
                WriteStream_.Dispose();
                WriteStream_ = null;

                string MD5 = CalcMD5(TempWritePath_);

                if (string.IsNullOrEmpty(MD5_) || MD5_ == MD5)
                {
                    if (CopyFile())
                    {
                        Done();
                    }
                    else
                    {
                        Abort(string.Format("copy file error : {0} -> {1}", TempWritePath_, WritePath_));
                    }
                }
                else
                {
                    File.Delete(TempWritePath_);
                    Abort("MD5 are different");
                }
            }
            else
            {
                Abort("download file size mismatching");
            }
        }

        private bool CopyFile()
        {
            try
            {
                if (File.Exists(WritePath_))
                {
                    File.Delete(WritePath_);
                }

                File.Move(TempWritePath_, WritePath_);
                return true;
            }
            catch(Exception Ex)
            {
                Abort("Exception CopyFile : " + Ex.Message);
                return false;
            }
        }

        private string CalcMD5(string FilePath)
        {
            try
            {
                FileStream Stream = new FileStream(FilePath, FileMode.Open);
                System.Security.Cryptography.MD5 MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] Data = MD5.ComputeHash(Stream);

                Stream.Close();
                Stream.Dispose();
                Stream = null;

                StringBuilder Result = new StringBuilder();
                for (int i = 0; i < Data.Length; i++)
                {
                    Result.Append(Data[i].ToString("X2"));
                }

                return Result.ToString();
            }
            catch(Exception Ex)
            {
                Abort("Exception CalcMD5 : " + Ex.Message);
                return string.Empty;
            }
        }
    }

    public static class Downloader
    {
        public const int MaxRetryCount = 5;
        public const int BufferSize = 4096;

        private static Queue<DownloadTask> DownloadTasks_ = new Queue<DownloadTask>();
        private static List<DownloadTask> FailedTasks_ = new List<DownloadTask>();
        private static DownloadTask CurrentTask_ = null;

        public static DownloadTask CurrentTask
        {
            get { return CurrentTask_; }
        }

        public static DownloadTask Start(string DownloadPath, string WritePath, string MD5, bool Overwrite)
        {
            DownloadTask Task = new DownloadTask(DownloadPath, WritePath, MD5, Overwrite, BufferSize);
            DownloadTasks_.Enqueue(Task);
            return Task;
        }

        public static void StopAllTask()
        {
            if (CurrentTask_ != null)
            {
                CurrentTask_.Stop();
                CurrentTask_ = null;
            }

            foreach (var Task in DownloadTasks_)
            {
                Task.Stop();
            }

            DownloadTasks_.Clear();
        }

        public static void Update(float DeltaTime)
        {
            if (CurrentTask_ == null && DownloadTasks_.Count == 0)
            {
                return;
            }

            if (CurrentTask_ != null)
            {
                CurrentTask_.Update(DeltaTime);
                CurrentTask_.InvokeProgressCallback();
                if (CurrentTask_.IsEnd())
                {
                    CurrentTask_.InvokeCompletedCallback();
                    if (!CurrentTask_.IsCompleted())
                    {
                        if (CurrentTask_.DownloadCount < MaxRetryCount)
                        {
                            CurrentTask_.Reset();
                            DownloadTasks_.Enqueue(CurrentTask_);
                        }
                        else
                        {
                            FailedTasks_.Add(CurrentTask_);
                        }
                    }

                    CurrentTask_ = null;
                }
            }
            else
            {
                CurrentTask_ = DownloadTasks_.Dequeue();
                CurrentTask_.Start();
            }
        }
    }
}