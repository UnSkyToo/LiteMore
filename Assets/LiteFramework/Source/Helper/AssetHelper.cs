using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Log;
using UnityEngine;
using UnityEngine.Networking;

namespace LiteFramework.Helper
{
    public static class AssetHelper
    {
        public const string FilesInfoFileName = "files.txt";

        public static void GenerateFilesInfoFile()
        {
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
            }

            var FilePath = Path.Combine(Application.dataPath, "Resources", FilesInfoFileName);
            var FileList = PathHelper.GetFileList(Application.streamingAssetsPath, (Path) => PathHelper.GetFileExt(Path) != ".meta");

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            using (var OutStream = new StreamWriter(FilePath, false, Encoding.ASCII))
            {
                foreach (var File in FileList)
                {
                    var RelativePath = File.Substring(Application.streamingAssetsPath.Length + 1);
                    OutStream.WriteLine(RelativePath);
                }

                OutStream.Close();
            }

            LLogger.LWarning($"Generate FilesInfo Succeed -> {FilePath}");
        }

        public static bool CacheStreamingAssets(Action<int, int, int> Callback)
        {
            var Files = Resources.Load<TextAsset>(PathHelper.GetFileNameWithoutExt(FilesInfoFileName));
            if (Files == null || string.IsNullOrWhiteSpace(Files.text))
            {
                LLogger.LWarning("version.txt is empty");
                return false;
            }

            var FileList = Files.text.Split(new char[] {'\r', '\n'});
            var FileListTrim = new List<string>();
            foreach (var FileName in FileList)
            {
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    continue;
                }

                FileListTrim.Add(FileName);
            }

            TaskManager.AddTask(CacheStreamingAssets(FileListTrim.ToArray(), Callback));
            return true;
        }

        private static IEnumerator CacheStreamingAssets(string[] FileList, Action<int, int, int> Callback)
        {
            var Index = 0;
            var DoneCount = 0;
            var ErrorCount = 0;
            while (Index < FileList.Length)
            {
                yield return CopyFileFromStreamingAssetsPathToPersistentDataPath(FileList[Index], (Suc) =>
                {
                    Index++;

                    if (Suc)
                    {
                        DoneCount++;
                    }
                    else
                    {
                        ErrorCount++;
                    }

                    Callback?.Invoke(ErrorCount, DoneCount, FileList.Length);
                });
            }

            Callback?.Invoke(ErrorCount, DoneCount, FileList.Length);
        }

        private static IEnumerator CopyFileFromStreamingAssetsPathToPersistentDataPath(string AssetBundlePath, Action<bool> Callback)
        {
            var Uri = new Uri(Path.Combine(Application.streamingAssetsPath, AssetBundlePath));
            using (var Request = new UnityWebRequest(Uri, "GET", (DownloadHandler)new DownloadHandlerBuffer(), (UploadHandler)null))
            {
                yield return Request.SendWebRequest();

                if (Request.isNetworkError || Request.isHttpError)
                {
                    LLogger.LWarning(Request.error);
                    Callback?.Invoke(false);
                }
                else
                {
                    if (Request.downloadHandler?.data == null)
                    {
                        LLogger.LWarning($"AssetBundle Data Is Null : {AssetBundlePath}");
                        Callback?.Invoke(false);
                    }
                    else
                    {
                        try
                        {
                            var TargetPath = Path.Combine(Application.persistentDataPath, AssetBundlePath);
                            var DirectoryPath = PathHelper.GetDirectory(TargetPath);
                            PathHelper.CreateDirectory(DirectoryPath);
                            File.WriteAllBytes(TargetPath, Request.downloadHandler.data);
                            Callback?.Invoke(true);
                        }
                        catch (LiteException Ex)
                        {
                            LLogger.LWarning(Ex.Message);
                            Callback?.Invoke(false);
                        }
                    }
                }
            }
        }
    }
}