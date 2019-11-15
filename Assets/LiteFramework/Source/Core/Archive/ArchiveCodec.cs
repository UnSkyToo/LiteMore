using System;
using System.IO;
using LiteFramework.Core.Log;
using LiteFramework.Core.Security;

namespace LiteFramework.Core.Archive
{
    public class ArchiveCodec
    {
        private const string Header = "lite";
        private const int ForceVersionCode = 1;
        private const int CurrentVersionCode = 1;

        private readonly string ArchivePath_;
        private readonly ArchiveReadStream ReadStream_;
        private readonly ArchiveDecoder Decoder_;
        private readonly ArchiveWriteStream WriteStream_;
        private readonly ArchiveEncoder Encoder_;

        public ArchiveCodec(string ArchivePath)
        {
            ArchivePath_ = ArchivePath;

            ReadStream_ = new ArchiveReadStream();
            Decoder_ = new ArchiveDecoder(ReadStream_);
            WriteStream_ = new ArchiveWriteStream();
            Encoder_ = new ArchiveEncoder(WriteStream_);
            WriteArchiveCrcConst();
        }

        public bool Load()
        {
            try
            {
                if (!ReadStream_.Load(ArchivePath_))
                {
                    return false;
                }
                if (!CheckArchiveCrc())
                {
                    return false;
                }

                Decoder_.Flush();
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                WriteArchiveCrcCode();
                return WriteStream_.Save(ArchivePath_);
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public bool Encode(IArchiveInfo Arc)
        {
            try
            {
                Arc.Encode(Encoder_);
                Encoder_.Flush();
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public bool Decode(IArchiveInfo Arc)
        {
            try
            {
                Arc.Decode(Decoder_);
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        private void WriteArchiveCrcConst()
        {
            // Header
            foreach (var Ch in Header)
            {
                WriteStream_.WriteInt8((byte)Ch);
            }

            // Version
            WriteStream_.WriteInt32(CurrentVersionCode);

            // Stream Length
            WriteStream_.WriteInt32(0);

            // Crc32
            WriteStream_.WriteUInt32(0);
        }

        private void WriteArchiveCrcCode()
        {
            var RawBuffer = WriteStream_.GetRawBuffer();

            // Stream Length
            WriteStream_.Seek(Header.Length + 4, SeekOrigin.Begin);
            WriteStream_.WriteInt32(RawBuffer.Length);

            // Crc32
            var DataStart = Header.Length + 4 + 4 + 4;
            var DataLength = RawBuffer.Length - DataStart;
            WriteStream_.WriteUInt32(Crc32.Calculate(RawBuffer, DataStart, DataLength));
        }

        private bool CheckArchiveCrc()
        {
            var RawBuffer = ReadStream_.GetRawBuffer();

            // Header
            foreach (var Ch in Header)
            {
                if (ReadStream_.ReadInt8() != (byte) Ch)
                {
                    LLogger.LError("is invalid lite archive file");
                    return false;
                }
            }

            // Version
            var VersionCode = ReadStream_.ReadInt32();
            if (VersionCode < ForceVersionCode)
            {
                LLogger.LError($"force require version {ForceVersionCode}, current is {VersionCode}");
                return false;
            }

            if (VersionCode < CurrentVersionCode)
            {
                // compatible code
            }

            // Length
            var Length = ReadStream_.ReadInt32();
            if (Length != RawBuffer.Length)
            {
                LLogger.LError($"error archive file length, expect : {Length}, but now : {RawBuffer.Length}");
                return false;
            }

            // Crc32
            var CrcCode = ReadStream_.ReadUInt32();
            var DataStart = Header.Length + 4 + 4 + 4;
            var DataLength = RawBuffer.Length - DataStart;
            var Code = Crc32.Calculate(RawBuffer, DataStart, DataLength);
            if (Code != CrcCode)
            {
                LLogger.LError("archive data is broken");
                return false;
            }

            return true;
        }
    }
}