using System;
using System.IO;
using System.Numerics;
using System.Text;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Archive
{
    public class ArchiveWriteStream
    {
        public long Length => Stream_.Length;
        private readonly MemoryStream Stream_;

        public ArchiveWriteStream()
        {
            Stream_ = new MemoryStream();
        }

        public bool Save(string FilePath)
        {
            try
            {
                File.WriteAllBytes(FilePath, GetRawBuffer());
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public void Seek(long Offset, SeekOrigin Origin)
        {
            Stream_.Seek(Offset, Origin);
        }

        public byte[] GetRawBuffer()
        {
            return Stream_.ToArray();
        }

        public void WriteBool(bool Value)
        {
            WriteInt8(Value ? (byte)1 : (byte)0);
        }

        public void WriteInt8(byte Value)
        {
            Stream_.WriteByte(Value);
        }

        public void WriteInt16(short Value)
        {
            Stream_.WriteByte((byte)((Value & 0x00ff)));
            Stream_.WriteByte((byte)((Value & 0xff00) >> 8));
        }

        public void WriteInt32(int Value)
        {
            Stream_.WriteByte((byte)((Value & 0x000000ff)));
            Stream_.WriteByte((byte)((Value & 0x0000ff00) >> 8));
            Stream_.WriteByte((byte)((Value & 0x00ff0000) >> 16));
            Stream_.WriteByte((byte)((Value & 0xff000000) >> 24));
        }

        public void WriteUInt32(uint Value)
        {
            Stream_.WriteByte((byte)((Value & 0x000000ff)));
            Stream_.WriteByte((byte)((Value & 0x0000ff00) >> 8));
            Stream_.WriteByte((byte)((Value & 0x00ff0000) >> 16));
            Stream_.WriteByte((byte)((Value & 0xff000000) >> 24));
        }

        public void WriteInt64(long Value)
        {
            Stream_.WriteByte((byte)(((ulong)Value & 0x00000000000000ff)));
            Stream_.WriteByte((byte)(((ulong)Value & 0x000000000000ff00) >> 8));
            Stream_.WriteByte((byte)(((ulong)Value & 0x0000000000ff0000) >> 16));
            Stream_.WriteByte((byte)(((ulong)Value & 0x00000000ff000000) >> 24));
            Stream_.WriteByte((byte)(((ulong)Value & 0x000000ff00000000) >> 32));
            Stream_.WriteByte((byte)(((ulong)Value & 0x0000ff0000000000) >> 40));
            Stream_.WriteByte((byte)(((ulong)Value & 0x00ff000000000000) >> 48));
            Stream_.WriteByte((byte)(((ulong)Value & 0xff00000000000000) >> 56));
        }

        public void WriteFloat(float Value)
        {
            var Buffer = BitConverter.GetBytes(Value);
            Stream_.Write(Buffer, 0, 4);
        }

        public void WriteDouble(double Value)
        {
            var Buffer = BitConverter.GetBytes(Value);
            Stream_.Write(Buffer, 0, 8);
        }

        public void WriteString(string Value)
        {
            var Buffer = Encoding.UTF8.GetBytes(Value);
            WriteInt16((short)Buffer.Length);
            Stream_.Write(Buffer, 0, (short)Buffer.Length);
        }

        public void WriteBigInteger(BigInteger Value)
        {
            WriteString(Value.ToString());
        }

        public void WriteArrayInt32(int[] Value)
        {
            if (Value == null)
            {
                WriteInt16(-1);
                return;
            }

            WriteInt16((short)Value.Length);

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                WriteInt32(Value[Index]);
            }
        }

        public void WriteArray2Int32(int[,] Value)
        {
            if (Value == null)
            {
                WriteInt16(-1);
                return;
            }

            WriteInt16((short)Value.GetLength(0));
            WriteInt16((short)Value.GetLength(1));

            for (var X = 0; X < Value.GetLength(0); ++X)
            {
                for (var Y = 0; Y < Value.GetLength(1); ++Y)
                {
                    WriteInt32(Value[X, Y]);
                }
            }
        }

        public void WriteArrayBigInteger(BigInteger[] Value)
        {
            if (Value == null)
            {
                WriteInt16(-1);
                return;
            }

            WriteInt16((short)Value.Length);

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                WriteBigInteger(Value[Index]);
            }
        }

        public void WriteArray2BigInteger(BigInteger[,] Value)
        {
            if (Value == null)
            {
                WriteInt16(-1);
                return;
            }

            WriteInt16((short)Value.GetLength(0));
            WriteInt16((short)Value.GetLength(1));

            for (var X = 0; X < Value.GetLength(0); ++X)
            {
                for (var Y = 0; Y < Value.GetLength(1); ++Y)
                {
                    WriteBigInteger(Value[X, Y]);
                }
            }
        }

        public void WriteArrayString(string[] Value)
        {
            if (Value == null)
            {
                WriteInt16(-1);
                return;
            }

            WriteInt16((short)Value.Length);

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                WriteString(Value[Index]);
            }
        }

        public void WriteStream(ArchiveWriteStream Stream)
        {
            var Len = (int)Stream.Length;
            WriteInt32(Len);
            Stream_.Write(Stream.GetRawBuffer(), 0, Len);
        }
    }
}