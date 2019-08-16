using System;
using System.Collections.Generic;
using LiteFramework.Core.ObjectPool;
using UnityEngine;
using UnityEngine.Profiling;

namespace LiteFramework.Extend.Debug
{
    internal abstract class DebuggerProfiler
    {
        internal class SummerItem : ScrollableDebuggerDrawItem
        {
            private const int OneMegaBytes = 1024 * 1024;

            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Profiler Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Supported:", Profiler.supported.ToString());
                    DrawItem("Enabled:", Profiler.enabled.ToString());
                    DrawItem("Enable Binary Log:", Profiler.enableBinaryLog ? $"True, {Profiler.logFile}" : "False");
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Area Count:", Profiler.areaCount.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                    DrawItem("Max Samples Number Per Frame:", Profiler.maxNumberOfSamplesPerFrame.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Max Used Memory:", Profiler.maxUsedMemory.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                    DrawItem("Mono Used Size:", $"{(Profiler.GetMonoUsedSizeLong() / (float)OneMegaBytes):F3} MB");
                    DrawItem("Mono Heap Size:", $"{(Profiler.GetMonoHeapSizeLong() / (float)OneMegaBytes):F3} MB");
                    DrawItem("Used Heap Size:", $"{(Profiler.usedHeapSizeLong / (float)OneMegaBytes):F3} MB");
                    DrawItem("Total Allocated Memory:",
                        $"{(Profiler.GetTotalAllocatedMemoryLong() / (float)OneMegaBytes):F3} MB");
                    DrawItem("Total Reserved Memory:",
                        $"{(Profiler.GetTotalReservedMemoryLong() / (float)OneMegaBytes):F3} MB");
                    DrawItem("Total Unused Reserved Memory:",
                        $"{(Profiler.GetTotalUnusedReservedMemoryLong() / (float)OneMegaBytes):F3} MB");
#else
                DrawItem("Mono Used Size:", $"{(Profiler.GetMonoUsedSize() / (float)OneMegaBytes):F3} MB");
                DrawItem("Mono Heap Size:", $"{(Profiler.GetMonoHeapSize() / (float)OneMegaBytes):F3} MB");
                DrawItem("Used Heap Size:", $"{(Profiler.usedHeapSize / (float)OneMegaBytes):F3} MB");
                DrawItem("Total Allocated Memory:", $"{(Profiler.GetTotalAllocatedMemory() / (float)OneMegaBytes):F3} MB");
                DrawItem("Total Reserved Memory:", $"{(Profiler.GetTotalReservedMemory() / (float)OneMegaBytes):F3} MB");
                DrawItem("Total Unused Reserved Memory:", $"{(Profiler.GetTotalUnusedReservedMemory() / (float)OneMegaBytes):F3} MB");
#endif

#if UNITY_2018_1_OR_NEWER
                    DrawItem("Allocated Memory For Graphics Driver:",
                        $"{(Profiler.GetAllocatedMemoryForGraphicsDriver() / (float)OneMegaBytes):F3} MB");
#endif
#if UNITY_5_5_OR_NEWER
                    DrawItem("Temp Allocator Size:",
                        $"{(Profiler.GetTempAllocatorSize() / (float)OneMegaBytes):F3} MB");
#endif
                }
                GUILayout.EndVertical();
            }
        }

        internal class MemorySummaryItem : ScrollableDebuggerDrawItem
        {
            private class Record
            {
                public string Name { get; }
                public int Count { get; set; }
                public long Size { get; set; }

                public Record(string Name)
                {
                    this.Name = Name;
                    this.Count = 0;
                    this.Size = 0L;
                }
            }

            private readonly List<Record> RecordList_ = new List<Record>();
            private DateTime SampleTime_ = DateTime.MinValue;
            private int SampleCount_ = 0;
            private long SampleSize_ = 0L;

            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Runtime Memory Summary</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    if (GUILayout.Button("Take Sample", GUILayout.Height(30f)))
                    {
                        TakeSample();
                    }

                    if (SampleTime_ <= DateTime.MinValue)
                    {
                        GUILayout.Label("<b>Please take sample first.</b>");
                    }
                    else
                    {
                        GUILayout.Label(
                            $"<b>{SampleCount_} Objects ({GetSizeString(SampleSize_)}) obtained at {SampleTime_.ToString("yyyy-MM-dd HH:mm:ss")}.</b>");

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("<b>Type</b>");
                            GUILayout.Label("<b>Count</b>", GUILayout.Width(120f));
                            GUILayout.Label("<b>Size</b>", GUILayout.Width(120f));
                        }
                        GUILayout.EndHorizontal();

                        foreach (var Rec in RecordList_)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Rec.Name);
                                GUILayout.Label(Rec.Count.ToString(), GUILayout.Width(120f));
                                GUILayout.Label(GetSizeString(Rec.Size), GUILayout.Width(120f));
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            private void TakeSample()
            {
                RecordList_.Clear();
                SampleTime_ = DateTime.Now;
                SampleCount_ = 0;
                SampleSize_ = 0L;

                var Samples = Resources.FindObjectsOfTypeAll<UnityEngine.Object>();
                foreach (var Sam in Samples)
                {
                    var SampleSize = 0L;
#if UNITY_5_6_OR_NEWER
                    SampleSize = Profiler.GetRuntimeMemorySizeLong(Sam);
#else
                    SampleSize = Profiler.GetRuntimeMemorySize(Sam);
#endif
                    var Name = Sam.GetType().Name;
                    SampleCount_++;
                    SampleSize_ += SampleSize;

                    Record CurRec = null;
                    foreach (var Rec in RecordList_)
                    {
                        if (Rec.Name == Name)
                        {
                            CurRec = Rec;
                            break;
                        }
                    }

                    if (CurRec == null)
                    {
                        CurRec = new Record(Name);
                        RecordList_.Add(CurRec);
                    }

                    CurRec.Count++;
                    CurRec.Size += SampleSize;
                }

                RecordList_.Sort(RecordComparer);
            }

            private string GetSizeString(long Size)
            {
                if (Size < 1024L)
                {
                    return $"{Size} Bytes";
                }

                if (Size < 1024L * 1024L)
                {
                    return $"{(Size / 1024f):F2} KB";
                }

                if (Size < 1024L * 1024L * 1024L)
                {
                    return $"{(Size / 1024f / 1024f):F2} MB";
                }

                if (Size < 1024L * 1024L * 1024L * 1024L)
                {
                    return $"{(Size / 1024f / 1024f / 1024f):F2} GB";
                }

                return $"{(Size / 1024f / 1024f / 1024f / 1024f):F2} TB";
            }

            private int RecordComparer(Record A, Record B)
            {
                var Result = B.Size.CompareTo(A.Size);
                if (Result != 0)
                {
                    return Result;
                }

                Result = A.Count.CompareTo(B.Count);
                if (Result != 0)
                {
                    return Result;
                }

                return A.Name.CompareTo(B.Name);
            }
        }

        internal class MemoryInfoItem<T> : ScrollableDebuggerDrawItem where T : UnityEngine.Object
        {
            private sealed class Sample
            {
                public string Name { get; }
                public string Type { get; }
                public long Size { get; }
                public bool Highlight { get; set; }

                public Sample(string Name, string Type, long Size)
                {
                    this.Name = Name;
                    this.Type = Type;
                    this.Size = Size;
                    this.Highlight = false;
                }
            }

            private const int ShowSampleCount = 300;

            private readonly List<Sample> SampleList_ = new List<Sample>();
            private DateTime SampleTime_ = DateTime.MinValue;
            private long SampleSize_ = 0L;
            private long DuplicateSampleSize_ = 0L;
            private int DuplicateSimpleCount_ = 0;

            protected override void OnDrawScrollable()
            {
                var TypeName = typeof(T).Name;
                GUILayout.Label($"<b>{TypeName} Runtime Memory Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    if (GUILayout.Button($"Take Sample for {TypeName}", GUILayout.Height(30f)))
                    {
                        TakeSample();
                    }

                    if (SampleTime_ <= DateTime.MinValue)
                    {
                        GUILayout.Label($"<b>Please take sample for {TypeName} first.</b>");
                    }
                    else
                    {
                        if (DuplicateSimpleCount_ > 0)
                        {
                            GUILayout.Label(
                                $"<b>{SampleList_.Count} {TypeName}s ({GetSizeString(SampleSize_)}) obtained at {SampleTime_.ToString("yyyy-MM-dd HH:mm:ss")}, while {DuplicateSimpleCount_} {TypeName}s ({GetSizeString(DuplicateSampleSize_)}) might be duplicated.</b>");
                        }
                        else
                        {
                            GUILayout.Label(
                                $"<b>{SampleList_.Count} {TypeName}s ({GetSizeString(SampleSize_)}) obtained at {SampleTime_.ToString("yyyy-MM-dd HH:mm:ss")}.</b>");
                        }

                        if (SampleList_.Count > 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"<b>{TypeName} Name</b>");
                                GUILayout.Label("<b>Type</b>", GUILayout.Width(240f));
                                GUILayout.Label("<b>Size</b>", GUILayout.Width(80f));
                            }
                            GUILayout.EndHorizontal();
                        }

                        var Count = 0;
                        foreach (var Sam in SampleList_)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Sam.Highlight ? $"<color=yellow>{Sam.Name}</color>" : $"{Sam.Name}");
                                GUILayout.Label(Sam.Highlight ? $"<color=yellow>{Sam.Type}</color>" : $"{Sam.Type}",
                                    GUILayout.Width(240f));
                                GUILayout.Label(
                                    Sam.Highlight
                                        ? $"<color=yellow>{GetSizeString(Sam.Size)}</color>"
                                        : $"{GetSizeString(Sam.Size)}", GUILayout.Width(80f));
                            }
                            GUILayout.EndHorizontal();

                            Count++;
                            if (Count >= ShowSampleCount)
                            {
                                break;
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            private void TakeSample()
            {
                SampleTime_ = DateTime.Now;
                SampleSize_ = 0L;
                DuplicateSampleSize_ = 0L;
                DuplicateSimpleCount_ = 0;
                SampleList_.Clear();

                var Samples = Resources.FindObjectsOfTypeAll<T>();
                foreach (var Sam in Samples)
                {
                    var MemSize = 0L;
#if UNITY_5_6_OR_NEWER
                    MemSize = Profiler.GetRuntimeMemorySizeLong(Sam);
#else
                    MemSize = Profiler.GetRuntimeMemorySize(Sam);
#endif
                    SampleSize_ += MemSize;
                    SampleList_.Add(new Sample(Sam.name, Sam.GetType().Name, MemSize));
                }

                SampleList_.Sort(SampleComparer);

                for (var Index = 1; Index < SampleList_.Count; ++Index)
                {
                    if (SampleList_[Index].Name == SampleList_[Index - 1].Name &&
                        SampleList_[Index].Type == SampleList_[Index - 1].Type &&
                        SampleList_[Index].Size == SampleList_[Index - 1].Size)
                    {
                        SampleList_[Index].Highlight = true;
                        DuplicateSampleSize_ += SampleList_[Index].Size;
                        DuplicateSimpleCount_++;
                    }
                }
            }

            private string GetSizeString(long Size)
            {
                if (Size < 1024L)
                {
                    return $"{Size} Bytes";
                }

                if (Size < 1024L * 1024L)
                {
                    return $"{(Size / 1024f):F2} KB";
                }

                if (Size < 1024L * 1024L * 1024L)
                {
                    return $"{(Size / 1024f / 1024f):F2} MB";
                }

                if (Size < 1024L * 1024L * 1024L * 1024L)
                {
                    return $"{(Size / 1024f / 1024f / 1024f):F2} GB";
                }

                return $"{(Size / 1024f / 1024f / 1024f / 1024f):F2} TB";
            }

            private int SampleComparer(Sample A, Sample B)
            {
                var Result = B.Size.CompareTo(A.Size);
                if (Result != 0)
                {
                    return Result;
                }

                Result = A.Type.CompareTo(B.Type);
                if (Result != 0)
                {
                    return Result;
                }

                return A.Name.CompareTo(B.Name);
            }
        }

        internal class ObjectPoolItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                var Pools = ObjectPoolManager.GetObjectPoolList();
                GUILayout.Label("<b>Object Pool Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Object Pool Count", Pools.Length.ToString());
                }
                GUILayout.EndVertical();
                foreach (var Pool in Pools)
                {
                    DrawObjectPool(Pool);
                }
            }

            private void DrawObjectPool(BaseObjectPool Pool)
            {
                GUILayout.Label($"<b>Object Pool: {Pool.PoolName}</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    var Caches = Pool.GetObjectCacheList();
                    DrawItem("Type", Pool.ObjectType.FullName);
                    DrawItem("Capacity", Pool.Capacity.ToString());
                    DrawItem("Used Count", Pool.UsedCount.ToString());
                    DrawItem("Idle Count", Pool.IdleCount.ToString());

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("<b>Is Used</b>", GUILayout.Width(80f));
                        GUILayout.Label("<b>Use Count</b>", GUILayout.Width(80f));
                        GUILayout.Label("<b>Use Time</b>", GUILayout.Width(80f));
                        GUILayout.Label("<b>Last Spawn Time</b>", GUILayout.Width(120f));
                        GUILayout.Label("<b>Last Recycle Time</b>", GUILayout.Width(120f));
                    }
                    GUILayout.EndHorizontal();

                    if (Caches.Length > 0)
                    {
                        foreach (var Cache in Caches)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Cache.Used.ToString(), GUILayout.Width(80f));
                                GUILayout.Label(Cache.Count.ToString(), GUILayout.Width(80f));
                                GUILayout.Label(Cache.TotalTime.ToString(), GUILayout.Width(80));
                                GUILayout.Label(Cache.LastSpawnTime.ToString("yyyy-MM-dd HH:mm:ss"), GUILayout.Width(120f));
                                GUILayout.Label(Cache.LastRecycleTime.ToString("yyyy-MM-dd HH:mm:ss"), GUILayout.Width(120f));
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.Label("<i>Object Pool is Empty ...</i>");
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}