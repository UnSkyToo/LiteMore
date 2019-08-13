using System.IO;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Lite.Framework.AppUpdate
{
    public enum StageID
    {
        Unkown = 0,
        Idle,
        ShowRetryInfo,
        Error,
        GetResVersion,
        DownloadResInfo,
        CheckResVersion,
        DownloadRes,
        CopyResInfoToDoc,
        Final,
    }

    public enum StageProgressID
    {
        Idle,
        Error,
        GetResVersionBegin,
        GetResVersionEnd,
        DownloadResInfoBegin,
        DownloadResInfoEnd,
        CheckResVersionBegin,
        CheckResVersionEnd,
        DownloadResBegin,
        DownloadResEnd,
        CopyResInfoToDocBegin,
        CopyResInfoToDocEnd,
        Done,
    }

    public enum StageCode
    {
        Failed,
        Succeeded,
        Continue,
    }

    public enum ErrorCode
    {
        Ok,
        GetResVersionInfoFailed,
        DownloadResInfoFailed,
        GetResPatchInfoListFailed,
        DownloadResFailed,
        CopyResInfoToDocFailed,
    }

    public class VersionInfo
    {
        public int Major_;
        public int Minor_;
        public int Build_;
        public int Revison_;

        public bool IsValid
        {
            get { return Major_ != -1; }
        }

        public VersionInfo(string VersionNumber)
        {
            try
            {
                string[] Params = VersionNumber.Split('.');
                Major_ = int.Parse(Params[0]);
                Minor_ = int.Parse(Params[1]);
                Build_ = int.Parse(Params[2]);
                Revison_ = int.Parse(Params[3]);
            }
            catch
            {
                Major_ = -1;
                Minor_ = 0;
                Build_ = 0;
                Revison_ = 0;
            }
        }

        public override bool Equals(object Obj)
        {
            if (Obj == null)
            {
                return false;
            }

            if (Obj.GetType() != GetType())
            {
                return false;
            }

            VersionInfo Info = (VersionInfo)Obj;
            return this == Info;
        }

        public override int GetHashCode()
        {
            return Major_.GetHashCode() + Minor_.GetHashCode() + Build_.GetHashCode() + Revison_.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major_, Minor_, Build_, Revison_);
        }

        public static bool operator ==(VersionInfo Lhs, VersionInfo Rhs)
        {
            if (object.Equals(Lhs, null) || object.Equals(Rhs, null))
            {
                return false;
            }

            if (Lhs.Revison_ == Rhs.Revison_ && Lhs.Build_ == Rhs.Build_ &&
                Lhs.Minor_ == Rhs.Minor_ && Lhs.Major_ == Rhs.Major_)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(VersionInfo Lhs, VersionInfo Rhs)
        {
            if (object.Equals(Lhs, null) || object.Equals(Rhs, null))
            {
                return true;
            }

            if (Lhs.Revison_ != Rhs.Revison_ || Lhs.Build_ != Rhs.Build_ ||
                Lhs.Minor_ != Rhs.Minor_ || Lhs.Major_ != Rhs.Major_)
            {
                return true;
            }

            return false;
        }
    }

    public class ResVersionInfo
    {
        public string ResUrl_;
        public VersionInfo Version_;
        
        public ResVersionInfo(string ResUrl, string VersionNumber)
        {
            Version_ = new VersionInfo(VersionNumber);
            ResUrl_ = ResUrl;
        }
    }

    public class ResPatchInfo
    {
        public VersionInfo From_;
        public VersionInfo To_;
        public string PatchUrl_;
        public string MD5_;
        public long Size_;

        public ResPatchInfo(string From, string To, string PatchUrl, string MD5, string Size)
        {
            From_ = new VersionInfo(From);
            To_ = new VersionInfo(To);
            PatchUrl_ = PatchUrl;
            MD5_ = MD5;
            Size_ = long.Parse(Size);
        }
    }

    public class ResPatchInfoList
    {
        public List<ResPatchInfo> PatchList_ = new List<ResPatchInfo>();

        public ResPatchInfoList()
        {
        }

        public ResPatchInfo GetPatchWithVersion(VersionInfo CurrentVersion)
        {
            foreach(ResPatchInfo PatchInfo in PatchList_)
            {
                if (PatchInfo.From_ == CurrentVersion)
                {
                    return PatchInfo;
                }
            }

            return null;
        }

        public void AddPatch(string From, string To, string PatchUrl, string MD5, string Size)
        {
            ResPatchInfo PatchInfo = new ResPatchInfo(From, To, PatchUrl, MD5, Size);
            PatchList_.Add(PatchInfo);
        }
    }

    public class UpdateTextInfo
    {
        private Dictionary<string, string> Texts_ = new Dictionary<string, string>();

        public string this[string Key]
        {
            get
            {
                if (Texts_.ContainsKey(Key))
                {
                    return Texts_[Key];
                }
                return string.Empty;
            }
            set
            {
                if (Texts_.ContainsKey(Key))
                {
                    Texts_[Key] = value;
                }
                else
                {
                    Texts_.Add(Key, value);
                }
            }
        }
    }

    public static class UpdateTool
    {
        public static string DataPath_ = Application.persistentDataPath;
        public static string VersionInfoName_ = "res_ver.xml";
        public static string VersionInfoPath_ = DataPath_ + "/" + VersionInfoName_;
        public static string ServerVersionInfoPath_ = DataPath_ + "/server_res.xml";

        public static ResVersionInfo GetResVersionInfo()
        {
            try
            {
                if (!File.Exists(VersionInfoPath_))
                {
                    CopyResVersionInfoToDoc();
                }

                XmlDocument Doc = new XmlDocument();
                Doc.Load(VersionInfoPath_);

                XmlNode ResVersionNode = Doc.SelectSingleNode("res_info/res_patch_ver");
                XmlNode ResUrlNode = Doc.SelectSingleNode("res_info/ver_url");

                ResVersionInfo Info = new ResVersionInfo(ResUrlNode.InnerText, ResVersionNode.InnerText);

                if (Info.Version_.IsValid)
                {
                    return Info;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static ResPatchInfoList GetResPatchInfoList()
        {
            try
            {
                ResPatchInfoList PatchInfoList = new ResPatchInfoList();

                XmlDocument Doc = new XmlDocument();
                Doc.Load(ServerVersionInfoPath_);

                XmlNodeList PatchList = Doc.SelectNodes("server_res_info/res_patch_ver");

                foreach (XmlNode Patch in PatchList)
                {
                    string From = Patch.SelectSingleNode("from").InnerText;
                    string To = Patch.SelectSingleNode("to").InnerText;
                    string PatchUrl = Patch.SelectSingleNode("patch").InnerText;
                    string MD5 = Patch.SelectSingleNode("md5").InnerText;
                    string Size = Patch.SelectSingleNode("size").InnerText;

                    PatchInfoList.AddPatch(From, To, PatchUrl, MD5, Size);
                }

                return PatchInfoList;
            }
            catch
            {
                return null;
            }
        }

        public static void UpdateResVersionInfo(VersionInfo Version)
        {
            if (!File.Exists(VersionInfoPath_))
            {
                return;
            }

            XmlDocument Doc = new XmlDocument();
            Doc.Load(VersionInfoPath_);

            XmlNode ResVersionNode = Doc.SelectSingleNode("res_info/res_patch_ver");
            ResVersionNode.InnerText = Version.ToString();

            Doc.Save(VersionInfoPath_);
        }

        public static UpdateTextInfo GetUpdateTextInfo()
        {
            UpdateTextInfo Info = new UpdateTextInfo();

            Info[ErrorCode.Ok.ToString()] = "成功";
            Info[ErrorCode.GetResVersionInfoFailed.ToString()] = "获取版本信息失败";
            Info[ErrorCode.DownloadResInfoFailed.ToString()] = "下载版本信息失败";
            Info[ErrorCode.GetResPatchInfoListFailed.ToString()] = "获取更新列表失败";
            Info[ErrorCode.DownloadResFailed.ToString()] = "下载补丁包失败";
            Info[ErrorCode.CopyResInfoToDocFailed.ToString()] = "解压缩补丁包失败";
            
            Info[StageProgressID.Idle.ToString()] = "空闲";
            Info[StageProgressID.Error.ToString()] = "错误";
            Info[StageProgressID.GetResVersionBegin.ToString()] = "开始获取版本信息";
            Info[StageProgressID.GetResVersionEnd.ToString()] = "获取版本信息结束";
            Info[StageProgressID.DownloadResInfoBegin.ToString()] = "开始下载版本信息";
            Info[StageProgressID.DownloadResInfoEnd.ToString()] = "下载版本信息结束";
            Info[StageProgressID.CheckResVersionBegin.ToString()] = "开始检查版本号";
            Info[StageProgressID.CheckResVersionEnd.ToString()] = "检查版本号结束";
            Info[StageProgressID.DownloadResBegin.ToString()] = "开始下载补丁包";
            Info[StageProgressID.DownloadResEnd.ToString()] = "下载补丁包结束";
            Info[StageProgressID.CopyResInfoToDocBegin.ToString()] = "开始解压缩补丁包";
            Info[StageProgressID.CopyResInfoToDocEnd.ToString()] = "解压缩补丁包结束";
            Info[StageProgressID.Done.ToString()] = "更新完成";

            return Info;
        }
        
        private static void CopyResVersionInfoToDoc()
        {
            // copy version info from app bundle
            TextAsset VersionInfoData = Resources.Load<TextAsset>(VersionInfoName_);
            if (VersionInfoData != null)
            {
                File.WriteAllBytes(VersionInfoPath_, VersionInfoData.bytes);
            }
        }
    }
}
