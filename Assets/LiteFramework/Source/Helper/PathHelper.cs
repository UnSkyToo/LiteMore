using System;
using System.Collections.Generic;
using System.IO;
using LiteFramework.Core.Log;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class PathHelper
    {
        public static bool PathIsFile(string FilePath)
        {
            return FilePath.LastIndexOf('.') > FilePath.LastIndexOf('/');
        }

        public static string UnifyPath(string FilePath)
        {
            FilePath = FilePath.Replace('\\', '/');
            // File
            if (PathIsFile(FilePath))
            {
                return FilePath;
            }

            // Directory
            if (!FilePath.EndsWith("/"))
            {
                FilePath += "/";
            }

            return FilePath;
        }

        public static string GetDirectory(string FilePath)
        {
            FilePath = UnifyPath(FilePath);

            if (PathIsFile(FilePath))
            {
                FilePath = FilePath.Substring(0, FilePath.LastIndexOf('/'));
            }

            return UnifyPath(FilePath);
        }


        public static string GetFileNameWithExt(string Path)
        {
            return System.IO.Path.GetFileName(Path);
        }

        public static string GetFileExt(string Path)
        {
            return System.IO.Path.GetExtension(Path);
        }

        public static string GetFileNameWithoutExt(string Path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public static string GetDirectoryAndFileName(string FilePath)
        {
            FilePath = UnifyPath(FilePath);

            if (PathIsFile(FilePath))
            {
                FilePath = FilePath.Substring(0, FilePath.LastIndexOf('.'));
            }

            return FilePath;
        }

        public static string GetAssetDataPath(string Path)
        {
            return $"{Application.dataPath}/{Path}";
        }

        public static string GetPersistentDataPath(string Path)
        {
            return $"{Application.persistentDataPath}/{Path}";
        }

        public static string GetStreamingAssetsPath(string Path)
        {
            return $"{Application.streamingAssetsPath}/{Path}";
        }

#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
        public static string GetStandaloneAssetsPath(string Path)
        {
            return $"{Application.dataPath}/{LiteConfigure.StandaloneAssetsName}/{Path}";
        }
#endif

        public static string GetAssetFullPath(string Path)
        {
            var FullPath = GetPersistentDataPath(Path);
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
            FullPath = GetStandaloneAssetsPath(Path);
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }
#endif

            return GetStreamingAssetsPath(Path);
        }

        public static List<string> GetFileList(string DirectoryPath, Func<string, bool> Filter = null)
        {
            var FileList = new List<string>();
            DirectoryPath = UnifyPath(DirectoryPath);

            var DirectoryAttrs = File.GetAttributes(DirectoryPath);
            if (((DirectoryAttrs & FileAttributes.Hidden) != 0) || ((DirectoryAttrs & FileAttributes.System) != 0) ||
                (((DirectoryAttrs & FileAttributes.Directory) == 0)))
            {
                return null;
            }

            var Files = Directory.GetFiles(DirectoryPath);
            var Directories = Directory.GetDirectories(DirectoryPath);

            if (Files != null && Files.Length > 0)
            {
                foreach (var SubFile in Files)
                {
                    var Attrs = File.GetAttributes(SubFile);
                    if (((Attrs & FileAttributes.Hidden) != 0) || ((Attrs & FileAttributes.System) != 0))
                    {
                        continue;
                    }

                    if (Filter != null && !Filter.Invoke(SubFile))
                    {
                        continue;
                    }

                    FileList.Add(UnifyPath(SubFile));
                }
            }

            if (Directories != null && Directories.Length > 0)
            {
                foreach (var SubDirectoryPath in Directories)
                {
                    var SubFileList = GetFileList(SubDirectoryPath, Filter);
                    if (SubFileList != null)
                    {
                        FileList.AddRange(SubFileList);
                    }
                }
            }

            return FileList;
        }

        public static bool CreateDirectory(string DirectoryPath)
        {
            DirectoryPath = UnifyPath(DirectoryPath);
            if (!Directory.Exists(DirectoryPath))
            {
                if (!Directory.CreateDirectory(DirectoryPath).Exists)
                {
                    LLogger.LWarning($"can't create directory : {DirectoryPath}");
                    return false;
                }
            }

            return true;
        }
    }
}