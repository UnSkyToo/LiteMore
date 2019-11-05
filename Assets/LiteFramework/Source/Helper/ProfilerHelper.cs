using System.Collections.Generic;
using LiteFramework.Core.Log;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LiteFramework.Helper
{
    public class ProfilerElement
    {
        public const int StatisticsCount = 60;

        private readonly string FullPath_;
        private readonly string ElementType_;
        private readonly double[] ElapsedTime_;
        private int Index_;
        private double AverageTime_;
        private double TotalTime_;
        private int Count_;
        private int MaxCount_;
        private float BeginTime_;
        private bool IsEnd_;

        private readonly Dictionary<string, ProfilerElement> ChildList_;

        public ProfilerElement(string FullPath, string ElementType)
        {
            FullPath_ = FullPath;
            ElementType_ = ElementType;
            ElapsedTime_ = new double[StatisticsCount];
            Index_ = 0;
            AverageTime_ = 0;
            TotalTime_ = 0;
            Count_ = 0;
            MaxCount_ = 0;
            BeginTime_ = 0;
            IsEnd_ = false;

            ChildList_ = new Dictionary<string, ProfilerElement>();
        }

        public string GetFullPath()
        {
            return FullPath_;
        }

        public string GetElementType()
        {
            return ElementType_;
        }

        public bool IsValid()
        {
            return IsEnd_;
        }

        public void Begin()
        {
            IsEnd_ = false;
            BeginTime_ = Time.realtimeSinceStartup;
        }

        public void End()
        {
            ElapsedTime_[Index_] += (Time.realtimeSinceStartup - BeginTime_);
            TotalTime_ += (Time.realtimeSinceStartup - BeginTime_);
            IsEnd_ = true;
        }

        public double GetAverageTime()
        {
            return AverageTime_;
        }

        public double GetTotalTime()
        {
            return TotalTime_;
        }

        public void Increase(int Count)
        {
            Count_ += Count;
        }

        public int GetCount()
        {
            return Count_;
        }

        public int GetMaxCount()
        {
            return MaxCount_;
        }

        public ProfilerElement GetChild(string Type)
        {
            if (ChildList_.ContainsKey(Type))
            {
                return ChildList_[Type];
            }

            return null;
        }

        public ProfilerElement AddChild(string Full, string Type)
        {
            if (!ChildList_.ContainsKey(Type))
            {
                ChildList_.Add(Type, new ProfilerElement(Full, Type));
            }

            return ChildList_[Type];
        }

        public void Tick()
        {
            for (var Index = 0; Index < StatisticsCount; ++Index)
            {
                AverageTime_ += ElapsedTime_[Index];
            }

            AverageTime_ /= (double)StatisticsCount;
            MaxCount_ = Count_ > MaxCount_ ? Count_ : MaxCount_;
            Count_ = 0;

            Index_++;
            if (Index_ >= StatisticsCount)
            {
                Index_ = 0;
            }
            ElapsedTime_[Index_] = 0;

            foreach (var Child in ChildList_)
            {
                Child.Value.Tick();
            }
        }

        public List<ProfilerElement> GetSortElements()
        {
            var Eles = new List<ProfilerElement>();
            Eles.AddRange(ChildList_.Values);
            Eles.Sort((A, B) =>
            {
                if (A.GetTotalTime() < B.GetTotalTime())
                {
                    return 1;
                }
                if (A.GetTotalTime() > B.GetTotalTime())
                {
                    return -1;
                }
                return 0;
            });
            return Eles;
        }

        public double SumTotalTime()
        {
            var Time = 0d;
            foreach (var Ele in ChildList_)
            {
                if (Ele.Value.IsValid())
                {
                    Time += Ele.Value.GetTotalTime();
                }
            }
            return Time;
        }

        public void Display()
        {
            DisplayMsg();

            foreach (var Ele in ChildList_)
            {
                Ele.Value.Display();
            }
        }

        public void DisplayMsg()
        {
            if (IsValid())
            {
                var Msg = $"[Profiler] {FullPath_}  ({GetTotalTime():0.000}s)";
                LLogger.LInfo(Msg);
            }
        }
    }

    public static class ProfilerHelper
    {
        public static bool Enable { get; set; }

        private static ProfilerElement Root_;

        static ProfilerHelper()
        {
            Enable = true;
            Clear();
        }

        public static void Clear()
        {
            Root_ = new ProfilerElement("Game", "Game");
        }

        public static ProfilerElement Get(string Path)
        {
            var Names = Path.Split('/');
            var Current = Root_;
            for (var Index = 0; Index < Names.Length; ++Index)
            {
                Current = Current.AddChild(Path, Names[Index]);
            }

            return Current;
        }

        public static void Begin()
        {
            Root_.Begin();
        }

        public static void Begin(string Name)
        {
            if (!Enable)
            {
                return;
            }

            Get(Name).Begin();
        }

        public static void End()
        {
            Root_.End();
        }

        public static void End(string Name)
        {
            if (!Enable)
            {
                return;
            }

            Get(Name).End();
        }

        public static double GetAverageTime(string Name)
        {
            return Get(Name).GetAverageTime();
        }

        public static double GetTotalTime()
        {
            return Root_.GetTotalTime();
        }

        public static double GetTotalTime(string Name)
        {
            return Get(Name).GetTotalTime();
        }

        public static double SumTotalTime()
        {
            return Root_.SumTotalTime();
        }

        public static List<ProfilerElement> GetSortElements()
        {
            return Root_.GetSortElements();
        }

        public static void Increase(string Name, int Count = 1)
        {
            if (!Enable)
            {
                return;
            }

            Get(Name).Increase(Count);
        }

        public static int GetCount(string Name)
        {
            return Get(Name).GetCount();
        }

        public static int GetMaxCount(string Name)
        {
            return Get(Name).GetMaxCount();
        }

        public static void Tick()
        {
            Root_.Tick();
        }

        public static void Display()
        {
            Root_.Display();
        }
    }

#if UNITY_EDITOR
    public class ProfilerHelperWindow : EditorWindow
    {
        public const int Width = 800;
        public const int Height = 600;
        private Vector2 ScrollPos_ = Vector2.zero;

        private void OnGUI()
        {
            var Elements = ProfilerHelper.GetSortElements();
            if (Elements.Count == 0)
            {
                EditorGUILayout.LabelField("Empty");
                return;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Profiler Elements({Elements.Count})");

            ScrollPos_ = EditorGUILayout.BeginScrollView(ScrollPos_);

            var TotalTime = Mathf.Clamp((float)ProfilerHelper.GetTotalTime(), 0.01f, float.MaxValue);
            EditorGUILayout.LabelField($"Total Time : {TotalTime:0.000}s");

            foreach (var Ele in Elements)
            {
                RenderElements(0, Ele, TotalTime);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void RenderElements(int Space, ProfilerElement Ele, double TotalTime)
        {
            if (Ele.IsValid())
            {
                var Percent = (Ele.GetTotalTime() / TotalTime);
                EditorGUILayout.BeginHorizontal();
                RenderSpace(Space);
                var Rect = GUILayoutUtility.GetRect(150, 20);
                EditorGUI.LabelField(Rect, $"{Ele.GetElementType()}");
                Rect = GUILayoutUtility.GetRect(400, 20);
                EditorGUI.ProgressBar(Rect, (float)Percent, $"{Percent * 100:0.00}%({Ele.GetTotalTime():0.000}s)");
                EditorGUILayout.EndHorizontal();

                var Childs = Ele.GetSortElements();
                if (Childs.Count > 0)
                {
                    foreach (var Child in Childs)
                    {
                        RenderElements(Space + 1, Child, Ele.GetTotalTime());
                    }
                }
            }
        }

        private void RenderSpace(int Space)
        {
            for (var I = 0; I < Space; ++I)
            {
                EditorGUILayout.Space();
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        [MenuItem("Lite/Profiler Helper Window")]
        private static void ShowWindow()
        {
            var Window = EditorWindow.GetWindow<ProfilerHelperWindow>(true, "Profiler Helper Window", true);
            Window.minSize = new Vector2(Width, Height);
            Window.maxSize = new Vector2(Width, Height);
            Window.position = new Rect((Screen.currentResolution.width - Width) / 2, (Screen.currentResolution.height - Height) / 2, Width, Height);
            Window.Show();
        }
    }
#endif
}