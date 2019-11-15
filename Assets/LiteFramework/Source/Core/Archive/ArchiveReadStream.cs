using System;
using System.IO;
using System.Numerics;
using System.Text;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Archive
{
    public class ArchiveReadStream
    {
        public long Length => Stream_.Length;

        private MemoryStream Stream_;
        private readonly byte[] Buffer_;

        public ArchiveReadStream()
        {
            Stream_ = new MemoryStream();
            Buffer_ = new byte[8];
        }

        public ArchiveReadStream(byte[] Buffer)
        {
            Stream_ = new MemoryStream(Buffer);
            Buffer_ = new byte[8];
        }

        public bool Load(string FilePath)
        {
            try
            {
                var Buffer = File.ReadAllBytes(FilePath);
                Stream_ = new MemoryStream(Buffer);
                Stream_.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public bool CanRead()
        {
            return Stream_.Position < Stream_.Length;
        }

        public void Seek(long Offset, SeekOrigin Origin)
        {
            Stream_.Seek(Offset, Origin);
        }

        public byte[] GetRawBuffer()
        {
            return Stream_.ToArray();
        }

        public bool ReadBool()
        {
            return Stream_.ReadByte() == 1;
        }

        public byte ReadInt8()
        {
            return (byte)Stream_.ReadByte();
        }

        public short ReadInt16()
        {
            Stream_.Read(Buffer_, 0, 2);
            return (short)((int)Buffer_[0] | ((int)Buffer_[1]) << 8);
        }

        public int ReadInt32()
        {
            Stream_.Read(Buffer_, 0, 4);
            return (int)((int)Buffer_[0] | ((int)Buffer_[1]) << 8 | ((int)Buffer_[2]) << 16 | ((int)Buffer_[3]) << 24);
        }

        public uint ReadUInt32()
        {
            Stream_.Read(Buffer_, 0, 4);
            return (uint)((uint)Buffer_[0] | ((uint)Buffer_[1]) << 8 | ((uint)Buffer_[2]) << 16 | ((uint)Buffer_[3]) << 24);
        }

        public long ReadInt64()
        {
            Stream_.Read(Buffer_, 0, 8);
            return (long)((long)Buffer_[0] | ((long)Buffer_[1]) << 8 | ((long)Buffer_[2]) << 16 | ((long)Buffer_[3]) << 24 |
                          (long)Buffer_[4] << 32 | ((long)Buffer_[5]) << 40 | ((long)Buffer_[6]) << 48 | ((long)Buffer_[7]) << 56);
        }

        public float ReadFloat()
        {
            Stream_.Read(Buffer_, 0, 4);
            return BitConverter.ToSingle(Buffer_, 0);
        }

        public double ReadDouble()
        {
            Stream_.Read(Buffer_, 0, 8);
            return BitConverter.ToDouble(Buffer_, 0);
        }

        public string ReadString()
        {
            var Len = (int)ReadInt16();
            var Buffer = new byte[Len];

            Stream_.Read(Buffer, 0, Len);
            return Encoding.UTF8.GetString(Buffer);
        }

        public BigInteger ReadBigInteger()
        {
            var Value = ReadString();
            return BigInteger.Parse(Value);
        }

        public int[] ReadArrayInt32()
        {
            var Count = ReadInt16();
            if (Count == -1)
            {
                return null;
            }

            var Value = new int[Count];

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                Value[Index] = ReadInt32();
            }

            return Value;
        }

        public int[,] ReadArray2Int32()
        {
            var Width = ReadInt16();
            if (Width == -1)
            {
                return null;
            }
            var Height = ReadInt16();

            var Value = new int[Width, Height];

            for (var X = 0; X < Width; ++X)
            {
                for (var Y = 0; Y < Height; ++Y)
                {
                    Value[X, Y] = ReadInt32();
                }
            }

            return Value;
        }

        public BigInteger[] ReadArrayBigInteger()
        {
            var Count = ReadInt16();
            if (Count == -1)
            {
                return null;
            }

            var Value = new BigInteger[Count];

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                Value[Index] = ReadBigInteger();
            }

            return Value;
        }

        public BigInteger[,] ReadArray2BigInteger()
        {
            var Width = ReadInt16();
            if (Width == -1)
            {
                return null;
            }
            var Height = ReadInt16();

            var Value = new BigInteger[Width, Height];

            for (var X = 0; X < Width; ++X)
            {
                for (var Y = 0; Y < Height; ++Y)
                {
                    Value[X, Y] = ReadBigInteger();
                }
            }

            return Value;
        }

        public string[] ReadArrayString()
        {
            var Count = ReadInt16();
            if (Count == -1)
            {
                return null;
            }

            var Value = new string[Count];

            for (var Index = 0; Index < Value.Length; ++Index)
            {
                Value[Index] = ReadString();
            }

            return Value;
        }

        public ArchiveReadStream ReadStream()
        {
            var Len = ReadInt32();
            var Buffer = new byte[Len];
            Stream_.Read(Buffer, 0, Len);
            return new ArchiveReadStream(Buffer);
        }
    }
}