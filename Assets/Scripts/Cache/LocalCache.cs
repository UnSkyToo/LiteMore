using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace LiteMore.Cache
{
    public static class LocalCache
    {
        private static readonly Dictionary<string, string> Buffer_ = new Dictionary<string, string>();

        public static bool ClearCache()
        {
            try
            {
                File.Delete(Configure.CacheFilePath);
                Buffer_.Clear();
                return true;
            }
            catch (Exception Ex)
            {
                Debug.LogError(Ex.Message);
                return false;
            }
        }

        public static bool LoadCache()
        {
            try
            {
                Buffer_.Clear();

                using (var InStream = new StreamReader(Configure.CacheFilePath, Encoding.ASCII))
                {
                    while (!InStream.EndOfStream)
                    {
                        var Line = InStream.ReadLine();
                        if (string.IsNullOrWhiteSpace(Line))
                        {
                            continue;
                        }

                        var Entity = Line.Split('`');
                        if (Entity.Length != 2)
                        {
                            throw new Exception($"Cache Line Error : {Line}");
                        }
                        Buffer_.Add(Entity[0], Entity[1]);
                    }
                    InStream.Close();
                }

                Debug.Log($"Load Cache Succeed : {Configure.CacheFilePath}");
                return true;
            }
            catch (Exception Ex)
            {
                Debug.LogWarning(Ex.Message);
                return false;
            }
        }

        public static bool SaveCache()
        {
            try
            {
                if (Buffer_.Count == 0)
                {
                    return true;
                }

                using (var OutStream = new StreamWriter(Configure.CacheFilePath, false, Encoding.ASCII))
                {
                    foreach (var Entity in Buffer_)
                    {
                        OutStream.WriteLine($"{Entity.Key}`{Entity.Value}");
                    }
                    OutStream.Close();
                }

                Debug.Log($"Save Cache Succeed : {Configure.CacheFilePath}");
                return true;
            }
            catch (Exception Ex)
            {
                Debug.LogWarning(Ex.Message);
                return false;
            }
        }

        public static string Get(string Key, string Default = "")
        {
            if (Buffer_.ContainsKey(Key))
            {
                return Buffer_[Key];
            }

            return Default;
        }

        public static void Set(string Key, string Value)
        {
            if (Buffer_.ContainsKey(Key))
            {
                Buffer_[Key] = Value;
            }
            else
            {
                Buffer_.Add(Key, Value);
            }
        }

        public static bool GetBoolean(string Key, bool Default = false)
        {
            try
            {
                if (Buffer_.ContainsKey(Key))
                {
                    return Convert.ToBoolean(Buffer_[Key]);
                }

                return Default;
            }
            catch
            {
                Debug.LogWarning($"Unknown Type (Not Boolean): {Key}");
                return Default;
            }
        }

        public static void SetBoolean(string Key, bool Value)
        {
            Set(Key, Value.ToString());
        }

        public static int GetInt(string Key, int Default = 0)
        {
            try
            {
                if (Buffer_.ContainsKey(Key))
                {
                    return Convert.ToInt32(Buffer_[Key]);
                }

                return Default;
            }
            catch
            {
                Debug.LogWarning($"Unknown Type (Not Int32): {Key}");
                return Default;
            }
        }

        public static void SetInt(string Key, int Value)
        {
            Set(Key, Value.ToString());
        }

        public static float GetFloat(string Key, float Default = 0.0f)
        {
            try
            {
                if (Buffer_.ContainsKey(Key))
                {
                    return Convert.ToSingle(Buffer_[Key]);
                }

                return Default;
            }
            catch
            {
                Debug.LogWarning($"Unknown Type (Not Float): {Key}");
                return Default;
            }
        }

        public static void SetFloat(string Key, float Value)
        {
            Set(Key, Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}