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

        public static string GetStandaloneAssetsPath(string Path)
        {
            return $"{Application.dataPath}/{LiteConfigure.StandaloneAssetsName}/{Path}";
        }

        public static string GetAssetFullPath(string Path)
        {
            var FullPath = GetPersistentDataPath(Path);
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

            FullPath = GetStreamingAssetsPath(Path);
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

#if UNITY_EDITOR
            FullPath = GetStandaloneAssetsPath(Path);
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

            if (System.IO.File.Exists(FullPath))
            {
                return Path;
            }
#endif

            return string.Empty;
        }
    }
}