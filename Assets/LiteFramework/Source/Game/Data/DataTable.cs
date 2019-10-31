using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using LiteFramework.Core.Log;
using LiteFramework.Helper;

namespace LiteFramework.Game.Data
{
    public class DataTable
    {
        public string Name { get; }
        public int LineCount => IsLoaded ? DataLines_.Count : 0;

        public bool IsLoaded { get; private set; }

        private int Index_;
        private byte[] Buffer_ = null;
        private readonly List<string> Keys_ = null;
        private readonly Dictionary<int, DataLine> DataLines_ = null;

        public DataTable(string Name)
        {
            this.Name = Name;
            this.IsLoaded = false;
            
            this.Keys_ = new List<string>();
            this.DataLines_ = new Dictionary<int, DataLine>();
        }

        public bool Parse(byte[] Buffer)
        {
            if (Buffer == null || Buffer.Length == 0)
            {
                return false;
            }

            try
            {
                IsLoaded = false;
                Keys_.Clear();
                DataLines_.Clear();

                Index_ = 0;
                Buffer_ = Buffer;

                var Column = ReadByte();
                for (var Index = 0; Index < Column; ++Index)
                {
                    var Key = ReadString2();
                    Keys_.Add(Key);
                }

                var HeaderType = new List<DataType>();

                for (var Index = 0; Index < Column; ++Index)
                {
                    var Type = ReadInt8();
                    HeaderType.Add((DataType)Type);
                }

                while (CanRead())
                {
                    var Line = new DataLine();

                    for (var Index = 0; Index < Column; ++Index)
                    {
                        switch (HeaderType[Index])
                        {
                            case DataType.Int:
                                Line.Add(Keys_[Index], new DataEntity<int>(ReadInt32()));
                                break;
                            case DataType.Short:
                                Line.Add(Keys_[Index], new DataEntity<short>(ReadInt16()));
                                break;
                            case DataType.Byte:
                                Line.Add(Keys_[Index], new DataEntity<byte>(ReadInt8()));
                                break;
                            case DataType.Bool:
                                Line.Add(Keys_[Index], new DataEntity<bool>(ReadBool()));
                                break;
                            case DataType.String:
                                Line.Add(Keys_[Index], new DataEntity<string>(ReadString2()));
                                break;
                            case DataType.BigInt:
                                Line.Add(Keys_[Index], new DataEntity<BigInteger>(BigNumHelper.ToBigInt(ReadString())));
                                break;
                            default:
                                break;
                        }
                    }

                    DataLines_.Add(Line.Get<int>("id"), Line);
                }

                IsLoaded = true;
                return true;
            }
            catch (Exception Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }

        public DataLine Line(int ID)
        {
            if (DataLines_.ContainsKey(ID))
            {
                return DataLines_[ID];
            }

            return null;
        }

        public int[] Keys()
        {
            return DataLines_.Keys.ToArray();
        }

        public T Value<T>(int ID, string Key)
        {
            if (DataLines_.ContainsKey(ID))
            {
                return DataLines_[ID].Get<T>(Key);
            }

            return default(T);
        }

        #region Data Reader
        private bool CanRead()
        {
            return Index_ < Buffer_.Length;
        }

        private byte ReadByte()
        {
            return Buffer_[Index_++];
        }

        private byte[] ReadByte(int Count)
        {
            var Data = new byte[Count];
            for (int Offset = 0; Offset < Count; ++Offset)
            {
                Data[Offset] = Buffer_[Index_ + Offset];
            }
            Index_ += Count;
            return Data;
        }

        private bool ReadBool()
        {
            var Value = ReadByte();

            if (Value == 0)
            {
                return false;
            }

            return true;
        }

        private byte ReadInt8()
        {
            return ReadByte();
        }

        private short ReadInt16()
        {
            var Data = ReadByte(2);
            var Value = (short)((int)Data[0] | (int)Data[1] << 8);
            return Value;
        }

        private int ReadInt32()
        {
            var Data = ReadByte(4);
            var Value = (int)((int)Data[0] | ((int)Data[1]) << 8 | ((int)Data[2]) << 16 | ((int)Data[3]) << 24);
            return Value;
        }

        private string ReadString()
        {
            var Length = ReadByte();
            var Data = ReadByte(Length);
            var Value = Encoding.UTF8.GetString(Data);
            return Value;
        }

        private string ReadString2()
        {
            var Length = ReadInt16();
            var Data = ReadByte(Length);
            var Value = Encoding.UTF8.GetString(Data);
            return Value;
        }

        private string ReadString4()
        {
            var Length = ReadInt32();
            var Data = ReadByte(Length);
            var Value = Encoding.UTF8.GetString(Data);
            return Value;
        }
        #endregion
    }
}