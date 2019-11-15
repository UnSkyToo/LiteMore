using System;
using System.Collections.Generic;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Archive
{
    public class ArchiveDecoder
    {
        private readonly ArchiveReadStream Stream_;
        private readonly Dictionary<string, object> CacheList_;

        public ArchiveDecoder(ArchiveReadStream Stream)
        {
            Stream_ = Stream;
            CacheList_ = new Dictionary<string, object>();
        }

        public T Read<T>(string Key, T Default)
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is T TValue)
            {
                return TValue;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        public T[] ReadArray<T>(string Key, T[] Default) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is object[] ArrayObj)
            {
                var OutputArray = new T[ArrayObj.Length];
                for (var Index = 0; Index < ArrayObj.Length; ++Index)
                {
                    OutputArray[Index] = (T) ArrayObj[Index];
                }
                return OutputArray;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        public T[,] ReadArray2<T>(string Key, T[,] Default) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is object[,] ArrayObj)
            {
                var Width = ArrayObj.GetLength(0);
                var Height = ArrayObj.GetLength(1);
                var OutputArray = new T[Width, Height];
                for (var X = 0; X < Width; ++X)
                {
                    for (var Y = 0; Y < Height; ++Y)
                    {
                        OutputArray[X, Y] = (T) ArrayObj[X, Y];
                    }
                }
                return OutputArray;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        public T ReadSub<T>(string Key) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return new T();
            }

            if (CacheList_[Key] is T Info)
            {
                return Info;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return new T();
        }

        public void Flush()
        {
            while (Stream_.CanRead())
            {
                var (Key, Value) = ReadFromStream(Stream_);
                if (string.IsNullOrWhiteSpace(Key))
                {
                    break;
                }

                CacheList_.Add(Key, Value);
            }
        }

        private (string, object) ReadFromStream(ArchiveReadStream Stream)
        {
            var Key = Stream.ReadString();
            var DataType = (ArchiveDataType) Stream.ReadInt8();

            switch (DataType)
            {
                case ArchiveDataType.Bool:
                    return (Key, Stream.ReadBool());
                case ArchiveDataType.Int8:
                    return (Key, Stream.ReadInt8());
                case ArchiveDataType.Int16:
                    return (Key, Stream.ReadInt16());
                case ArchiveDataType.Int32:
                    return (Key, Stream.ReadInt32());
                case ArchiveDataType.UInt32:
                    return (Key, Stream.ReadUInt32());
                case ArchiveDataType.Int64:
                    return (Key, Stream.ReadInt64());
                case ArchiveDataType.Float:
                    return (Key, Stream.ReadFloat());
                case ArchiveDataType.Double:
                    return (Key, Stream.ReadDouble());
                case ArchiveDataType.String:
                    return (Key, Stream.ReadString());
                case ArchiveDataType.BigInteger:
                    return (Key, Stream.ReadBigInteger());
                case ArchiveDataType.ArrayInt32:
                    return (Key, Stream.ReadArrayInt32());
                case ArchiveDataType.Array2Int32:
                    return (Key, Stream.ReadArray2Int32());
                case ArchiveDataType.ArrayBigInteger:
                    return (Key, Stream.ReadArrayBigInteger());
                case ArchiveDataType.Array2BigInteger:
                    return (Key, Stream.ReadArray2BigInteger());
                case ArchiveDataType.ArrayString:
                    return (Key, Stream.ReadArrayString());
                case ArchiveDataType.Array:
                    var Count = Stream.ReadInt16();
                    if (Count == -1)
                    {
                        return (Key, null);
                    }

                    var ArrayT = Type.GetType(Stream.ReadString());
                    var ArrayValue = new object[Count];

                    for (var Index = 0; Index < Count; ++Index)
                    {
                        if (Stream.ReadBool() == false)
                        {
                            ArrayValue[Index] = default;
                        }
                        else
                        {
                            ArrayValue[Index] = ReadFromSubStream(Stream, ArrayT);
                        }
                    }

                    return (Key, ArrayValue);
                case ArchiveDataType.Array2:
                    var Width = Stream.ReadInt16();
                    if (Width == 0)
                    {
                        return (Key, null);
                    }
                    var Height = Stream.ReadInt16();

                    var Array2T = Type.GetType(Stream.ReadString());
                    var Array2Value = new object[Width, Height];

                    for (var X = 0; X < Width; ++X)
                    {
                        for (var Y = 0; Y < Height; ++Y)
                        {
                            if (Stream.ReadBool() == false)
                            {
                                Array2Value[X, Y] = default;
                            }
                            else
                            {
                                Array2Value[X, Y] = ReadFromSubStream(Stream, Array2T);
                            }
                        }
                    }

                    return (Key, Array2Value);
                case ArchiveDataType.Sub:
                    var SubType = Type.GetType(Stream.ReadString());
                    return (Key, ReadFromSubStream(Stream, SubType));
                default:
                    break;
            }

            return (string.Empty, null);
        }

        private IArchiveInfo ReadFromSubStream(ArchiveReadStream MainStream, Type SubType)
        {
            var SubStream = MainStream.ReadStream();
            var SubEncoder = new ArchiveDecoder(SubStream);
            SubEncoder.Flush();
            var SubInfo = Activator.CreateInstance(SubType) as IArchiveInfo;
            SubInfo.Decode(SubEncoder);
            return SubInfo;
        }
    }
}