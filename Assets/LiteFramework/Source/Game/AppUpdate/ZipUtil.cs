using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

namespace Lite.Framework.AppUpdate
{
    public static class ZipUtil
    {
        public static readonly int BufferSize = 4096;

        /// <summary>
        /// 压缩单个文件到数据流
        /// </summary>
        /// <param name="outStream">输出数据流</param>
        /// <param name="filePath">需要压缩的文件路径</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipLevel">压缩等级（0-9）</param>
        public static void ZipFileToStream(Stream outStream, string filePath, string password, int zipLevel)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception("文件" + filePath + "不存在");
                }

                using (FileStream inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fileInfo = new FileInfo(filePath);

                    ZipOutputStream zipStream = new ZipOutputStream(outStream);
                    zipLevel = zipLevel > 9 ? 9 : zipLevel < 0 ? 0 : zipLevel;
                    zipStream.SetLevel(zipLevel);
                    zipStream.Password = password;

                    byte[] buffer = new byte[BufferSize];
                    int size = 0;

                    ZipEntry zipEntry = new ZipEntry(Path.GetFileName(filePath));
                    zipEntry.DateTime = fileInfo.CreationTime > fileInfo.LastWriteTime ? fileInfo.LastWriteTime : fileInfo.CreationTime;
                    zipEntry.Size = fileInfo.Length;
                    zipStream.PutNextEntry(zipEntry);

                    while(true)
                    {
                        size = inStream.Read(buffer, 0, BufferSize);

                        if (size <= 0)
                        {
                            break;
                        }

                        zipStream.Write(buffer, 0, size);
                    }

                    zipStream.Finish();
                    inStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="zipFilePath">压缩后的文件路径</param>
        /// <param name="filePath">需要压缩的文件路径</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipLevel">压缩等级（0-9）</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        public static void ZipFile(string zipFilePath, string filePath, string password, int zipLevel, bool overwrite)
        {
            try
            {
                if (File.Exists(zipFilePath) && !overwrite)
                {
                    return;
                }

                using(FileStream outStream = new FileStream(zipFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    ZipFileToStream(outStream, filePath, password, zipLevel);
                    outStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 压缩目录到Zip压缩流
        /// </summary>
        /// <param name="outStream">Zip压缩流</param>
        /// <param name="directoryPath">压缩目录</param>
        /// <param name="parentPath">相对父级目录</param>
        private static void ZipDirectoryToZipStream(ZipOutputStream outStream, string directoryPath, string parentPath)
        {
            try
            {
                if (directoryPath == string.Empty)
                {
                    return;
                }
                
                if (directoryPath[directoryPath.Length - 1] != '/')
                {
                    directoryPath += '/';
                }

                if (parentPath[parentPath.Length - 1] != '/')
                {
                    parentPath += '/';
                }
                
                ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
                string[] filePaths = Directory.GetFileSystemEntries(directoryPath);

                foreach (string path in filePaths)
                {
                    var filePath = path.Replace('\\', '/');
                    if (Directory.Exists(filePath)) // 是目录
                    {
                        string pPath = parentPath + filePath.Substring(filePath.LastIndexOf('/') + 1);
                        pPath += '/';
                        ZipDirectoryToZipStream(outStream, filePath, pPath);
                    }
                    else
                    {
                        using (FileStream inStream = File.OpenRead(filePath))
                        {
                            byte[] buffer = new byte[inStream.Length];
                            inStream.Read(buffer, 0, buffer.Length);
                            inStream.Close();

                            crc.Reset();
                            crc.Update(buffer);

                            string entryPath = parentPath + filePath.Substring(filePath.LastIndexOf('/') + 1);
                            ZipEntry zipEntry = new ZipEntry(entryPath);
                            FileInfo fileInfo = new FileInfo(filePath);

                            zipEntry.DateTime = fileInfo.CreationTime > fileInfo.LastWriteTime ? fileInfo.LastWriteTime : fileInfo.CreationTime;
                            zipEntry.Size = fileInfo.Length;
                            zipEntry.Crc = crc.Value;

                            outStream.PutNextEntry(zipEntry);
                            outStream.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 压缩目录到数据流
        /// </summary>
        /// <param name="outStream">输出数据流</param>
        /// <param name="directoryPath">压缩目录</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipLevel">压缩等级（0-9）</param>
        public static void ZipDirectoryToStream(Stream outStream, string directoryPath, string password, int zipLevel)
        {
            try
            {
                ZipOutputStream zipStream = new ZipOutputStream(outStream);
                zipLevel = zipLevel > 9 ? 9 : zipLevel < 0 ? 0 : zipLevel;
                zipStream.SetLevel(zipLevel);
                zipStream.Password = password;
                ZipDirectoryToZipStream(zipStream, directoryPath, string.Empty);
                zipStream.Finish();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 压缩多个目录到数据流
        /// </summary>
        /// <param name="outStream">输出数据流</param>
        /// <param name="parentPath">父目录</param>
        /// <param name="directories">压缩的目录列表</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipLevel">压缩等级（0-9）</param>
        public static void ZipDirectoriesToStream(Stream outStream, string parentPath, string[] directories, string password, int zipLevel)
        {
            try
            {
                ZipOutputStream zipStream = new ZipOutputStream(outStream);
                zipLevel = zipLevel > 9 ? 9 : zipLevel < 0 ? 0 : zipLevel;
                zipStream.SetLevel(zipLevel);
                zipStream.Password = password;

                foreach (var directoryPath in directories)
                {
                    ZipDirectoryToZipStream(zipStream, directoryPath, directoryPath.Substring(parentPath.Length + 1));
                }

                zipStream.Finish();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="directoryPath">压缩目录</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipLevel">压缩等级（0-9）</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        public static void ZipDirectory(string zipFilePath, string directoryPath, string password, int zipLevel, bool overwrite)
        {
            try
            {
                if (File.Exists(zipFilePath) && !overwrite)
                {
                    return;
                }

                using(FileStream outStream = new FileStream(zipFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    ZipDirectoryToStream(outStream, directoryPath, password, zipLevel);
                    outStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压缩单个文件
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unzipDirectoryPath">解压目录（为空则和压缩文件同级目录）</param>
        /// <param name="password">解压密码</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        public static void UnZipFile(string zipFilePath, string unzipDirectoryPath, string password, bool overwrite)
        {
            try
            {
                if (!File.Exists(zipFilePath))
                {
                    throw new Exception("文件" + zipFilePath + "不存在");
                }

                if (unzipDirectoryPath == string.Empty)
                {
                    unzipDirectoryPath = zipFilePath.Substring(0, zipFilePath.LastIndexOf('/') + 1);

                    if (unzipDirectoryPath == string.Empty)
                    {
                        unzipDirectoryPath = Directory.GetCurrentDirectory();
                    }
                }
                if (!unzipDirectoryPath.EndsWith("/"))
                {
                    unzipDirectoryPath += "/";
                }

                using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    zipStream.Password = password;
                    ZipEntry zipEntry = null;

                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (zipEntry.Name == string.Empty)
                        {
                            continue;
                        }

                        string directoryPath = Path.GetDirectoryName(zipEntry.Name);

                        Directory.CreateDirectory(unzipDirectoryPath + directoryPath);

                        if (!zipEntry.IsDirectory)
                        {
                            if (!File.Exists(unzipDirectoryPath + zipEntry.Name) || overwrite)
                            {
                                using (FileStream outStream = File.Create(unzipDirectoryPath + zipEntry.Name))
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    int size = 0;

                                    while(true)
                                    {
                                        size = zipStream.Read(buffer, 0, BufferSize);

                                        if (size <= 0)
                                        {
                                            break;
                                        }

                                        outStream.Write(buffer, 0, size);
                                    }

                                    outStream.Close();
                                }
                            }
                        }
                    }

                    zipStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压缩单个文件
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unzipDirectoryPath">解压目录（为空则和压缩文件同级目录）</param>
        /// <param name="password">解压密码</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        /// <param name="progressCallback">进度回调</param>
        public static void UnZipFileEx(string zipFilePath, string unzipDirectoryPath, string password, bool overwrite, Action<string, long, long> progressCallback)
        {
            try
            {
                if (!File.Exists(zipFilePath))
                {
                    throw new Exception("文件" + zipFilePath + "不存在");
                }

                if (unzipDirectoryPath == string.Empty)
                {
                    unzipDirectoryPath = zipFilePath.Substring(0, zipFilePath.LastIndexOf('/') + 1);

                    if (unzipDirectoryPath == string.Empty)
                    {
                        unzipDirectoryPath = Directory.GetCurrentDirectory();
                    }
                }
                if (!unzipDirectoryPath.EndsWith("/"))
                {
                    unzipDirectoryPath += "/";
                }

                using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    zipStream.Password = password;
                    ZipEntry zipEntry = null;

                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (zipEntry.Name == string.Empty)
                        {
                            continue;
                        }

                        string directoryPath = Path.GetDirectoryName(zipEntry.Name);

                        Directory.CreateDirectory(unzipDirectoryPath + directoryPath);

                        if (!zipEntry.IsDirectory)
                        {
                            if (!File.Exists(unzipDirectoryPath + zipEntry.Name) || overwrite)
                            {
                                using (FileStream outStream = File.Create(unzipDirectoryPath + zipEntry.Name))
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    int size = 0;
                                    long maxSize = zipStream.Length;
                                    long curSize = size;

                                    while (true)
                                    {
                                        size = zipStream.Read(buffer, 0, BufferSize);

                                        if (size <= 0)
                                        {
                                            break;
                                        }

                                        outStream.Write(buffer, 0, size);

                                        curSize += size;
                                        if (progressCallback != null)
                                        {
                                            progressCallback(zipEntry.Name, curSize, maxSize);
                                        }
                                    }

                                    outStream.Close();
                                }
                            }
                        }
                    }

                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
