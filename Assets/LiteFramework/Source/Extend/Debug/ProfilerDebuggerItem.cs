using System;
using System.Collections.Generic;
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
                        GUILayout.Label($"<b>{SampleCount_} Objects ({GetSizeString(SampleSize_)}) obtained at {SampleTime_.ToString("yyyy-MM-dd HH:mm:ss")}.</b>");

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

            private readonly List<Sample> m_Samples = new List<Sample>();
            private DateTime m_SampleTime = DateTime.MinValue;
            private long m_SampleSize = 0L;
            private long m_DuplicateSampleSize = 0L;
            private int m_DuplicateSimpleCount = 0;

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

                    if (m_SampleTime <= DateTime.MinValue)
                    {
                        GUILayout.Label($"<b>Please take sample for {TypeName} first.</b>");
                    }
                    else
                    {
                        if (m_DuplicateSimpleCount > 0)
                        {
                            GUILayout.Label($"<b>{m_Samples.Count} {TypeName}s ({GetSizeString(m_SampleSize)}) obtained at {m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss")}, while {m_DuplicateSimpleCount} {TypeName}s ({GetSizeString(m_DuplicateSampleSize)}) might be duplicated.</b>");
                        }
                        else
                        {
                            GUILayout.Label($"<b>{m_Samples.Count} {TypeName}s ({GetSizeString(m_SampleSize)}) obtained at {m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss")}.</b>");
                        }

                        if (m_Samples.Count > 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"<b>{TypeName} Name</b>");
                                GUILayout.Label("<b>Type</b>", GUILayout.Width(240f));
                                GUILayout.Label("<b>Size</b>", GUILayout.Width(80f));
                            }
                            GUILayout.EndHorizontal();
                        }

                        int count = 0;
                        for (int i = 0; i < m_Samples.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(m_Samples[i].Highlight ? $"<color=yellow>{m_Samples[i].Name}</color>" : $"{m_Samples[i].Name}");
                                GUILayout.Label(m_Samples[i].Highlight ? $"<color=yellow>{m_Samples[i].Type}</color>" : $"{m_Samples[i].Type}", GUILayout.Width(240f));
                                GUILayout.Label(m_Samples[i].Highlight ? $"<color=yellow>{GetSizeString(m_Samples[i].Size)}</color>" : $"{GetSizeString(m_Samples[i].Size)}", GUILayout.Width(80f));
                            }
                            GUILayout.EndHorizontal();

                            count++;
                            if (count >= ShowSampleCount)
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
                m_SampleTime = DateTime.Now;
                m_SampleSize = 0L;
                m_DuplicateSampleSize = 0L;
                m_DuplicateSimpleCount = 0;
                m_Samples.Clear();

                T[] samples = Resources.FindObjectsOfTypeAll<T>();
                for (int i = 0; i < samples.Length; i++)
                {
                    long sampleSize = 0L;
#if UNITY_5_6_OR_NEWER
                    sampleSize = Profiler.GetRuntimeMemorySizeLong(samples[i]);
#else
                    sampleSize = Profiler.GetRuntimeMemorySize(samples[i]);
#endif
                    m_SampleSize += sampleSize;
                    m_Samples.Add(new Sample(samples[i].name, samples[i].GetType().Name, sampleSize));
                }

                m_Samples.Sort(SampleComparer);

                for (int i = 1; i < m_Samples.Count; i++)
                {
                    if (m_Samples[i].Name == m_Samples[i - 1].Name && m_Samples[i].Type == m_Samples[i - 1].Type && m_Samples[i].Size == m_Samples[i - 1].Size)
                    {
                        m_Samples[i].Highlight = true;
                        m_DuplicateSampleSize += m_Samples[i].Size;
                        m_DuplicateSimpleCount++;
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
                int result = B.Size.CompareTo(A.Size);
                if (result != 0)
                {
                    return result;
                }

                result = A.Type.CompareTo(B.Type);
                if (result != 0)
                {
                    return result;
                }

                return A.Name.CompareTo(B.Name);
            }
        }
    }
}