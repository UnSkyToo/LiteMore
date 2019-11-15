using System.Collections.Generic;
using System.Numerics;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Archive
{
    public class ArchiveEncoder
    {
        private readonly ArchiveWriteStream Stream_;
        private readonly Dictionary<string, object> CacheList_;

        public ArchiveEncoder(ArchiveWriteStream Stream)
        {
            Stream_ = Stream;
            CacheList_ = new Dictionary<string, object>();
        }

        public void Write<T>(string Key, T Value)
        {
            if (CacheList_.ContainsKey(Key))
            {
                LLogger.LWarning($"archive repeat key : {Key}");
                CacheList_[Key] = Value;
                return;
            }

            CacheList_.Add(Key, Value);
        }

        public void WriteArray<T>(string Key, T[] Value) where T : IArchiveInfo, new()
        {
            if (CacheList_.ContainsKey(Key))
            {
                LLogger.LWarning($"archive repeat key : {Key}");
                CacheList_[Key] = Value;
                return;
            }

            CacheList_.Add(Key, Value);
        }

        public void WriteArray2<T>(string Key, T[,] Value) where T : IArchiveInfo, new()
        {
            if (CacheList_.ContainsKey(Key))
            {
                LLogger.LWarning($"archive repeat key : {Key}");
                CacheList_[Key] = Value;
                return;
            }

            CacheList_.Add(Key, Value);
        }

        public void WriteSub<T>(string Key, T Value) where T : IArchiveInfo, new()
        {
            if (CacheList_.ContainsKey(Key))
            {
                LLogger.LWarning($"archive repeat key : {Key}");
                CacheList_[Key] = Value;
                return;
            }

            CacheList_.Add(Key, Value);
        }

        public void Flush()
        {
            foreach (var Data in CacheList_)
            {
                WriteToStream(Stream_, Data.Key, Data.Value);
            }
        }
        
        private void WriteToStream(ArchiveWriteStream Stream, string Key, object Value)
        {
            Stream.WriteString(Key);

            switch (Value)
            {
                case bool BoolData:
                    Stream.WriteInt8((byte)ArchiveDataType.Bool);
                    Stream.WriteBool(BoolData);
                    break;
                case byte Int8Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Int8);
                    Stream.WriteInt8(Int8Data);
                    break;
                case short Int16Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Int16);
                    Stream.WriteInt16(Int16Data);
                    break;
                case int Int32Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Int32);
                    Stream.WriteInt32(Int32Data);
                    break;
                case uint UInt32Data:
                    Stream.WriteInt8((byte)ArchiveDataType.UInt32);
                    Stream.WriteUInt32(UInt32Data);
                    break;
                case long Int64Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Int64);
                    Stream.WriteInt64(Int64Data);
                    break;
                case float FloatData:
                    Stream.WriteInt8((byte)ArchiveDataType.Float);
                    Stream.WriteFloat(FloatData);
                    break;
                case double DoubleData:
                    Stream.WriteInt8((byte)ArchiveDataType.Double);
                    Stream.WriteDouble(DoubleData);
                    break;
                case string StringData:
                    Stream.WriteInt8((byte)ArchiveDataType.String);
                    Stream.WriteString(StringData);
                    break;
                case BigInteger BigIntegerData:
                    Stream.WriteInt8((byte)ArchiveDataType.BigInteger);
                    Stream.WriteBigInteger(BigIntegerData);
                    break;
                case int[] ArrayInt32Data:
                    Stream.WriteInt8((byte)ArchiveDataType.ArrayInt32);
                    Stream.WriteArrayInt32(ArrayInt32Data);
                    break;
                case int[,] Array2Int32Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Array2Int32);
                    Stream.WriteArray2Int32(Array2Int32Data);
                    break;
                case BigInteger[] ArrayBigIntegerData:
                    Stream.WriteInt8((byte)ArchiveDataType.ArrayBigInteger);
                    Stream.WriteArrayBigInteger(ArrayBigIntegerData);
                     break;
                case BigInteger[,] Array2BigIntegerData:
                    Stream.WriteInt8((byte)ArchiveDataType.Array2BigInteger);
                    Stream.WriteArray2BigInteger(Array2BigIntegerData);
                    break;
                case string[] ArrayStringData:
                    Stream.WriteInt8((byte)ArchiveDataType.ArrayString);
                    Stream.WriteArrayString(ArrayStringData);
                    break;
                case object[] ArrayData:
                    Stream.WriteInt8((byte)ArchiveDataType.Array);

                    Stream.WriteInt16((short)ArrayData.Length);
                    Stream.WriteString(ArrayData.GetType().GetElementType().FullName);

                    for (var Index = 0; Index < ArrayData.Length; ++Index)
                    {
                        var Info = ArrayData[Index] as IArchiveInfo;
                        Stream.WriteBool(Info != null);
                        if (Info != null)
                        {
                            WriteToSubStream(Stream, Info);
                        }
                    }
                    break;
                case object[,] Array2Data:
                    Stream.WriteInt8((byte)ArchiveDataType.Array2);

                    Stream.WriteInt16((short)Array2Data.GetLength(0));
                    Stream.WriteInt16((short)Array2Data.GetLength(1));
                    Stream.WriteString(Array2Data.GetType().GetElementType().FullName);

                    for (var X = 0; X < Array2Data.GetLength(0); ++X)
                    {
                        for (var Y = 0; Y < Array2Data.GetLength(1); ++Y)
                        {
                            var Info = Array2Data[X, Y] as IArchiveInfo;
                            Stream.WriteBool(Info != null);
                            if (Info != null)
                            {
                                WriteToSubStream(Stream, Info);
                            }
                        }
                    }
                    break;
                case null:
                    Stream.WriteInt8((byte)ArchiveDataType.Array);
                    Stream.WriteInt16(-1);
                    break;
                case IArchiveInfo Info:
                    Stream.WriteInt8((byte)ArchiveDataType.Sub);
                    Stream.WriteString(Info.GetType().FullName);
                    WriteToSubStream(Stream, Info);
                    break;
                default:
                    Stream.WriteInt8((byte)ArchiveDataType.Error);
                    break;
            }
        }

        private void WriteToSubStream(ArchiveWriteStream MainStream, IArchiveInfo Info)
        {
            var SubStream = new ArchiveWriteStream();
            var SubEncoder = new ArchiveEncoder(SubStream);
            Info.Encode(SubEncoder);
            SubEncoder.Flush();
            MainStream.WriteStream(SubStream);
        }
    }
}