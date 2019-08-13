using System.Collections.Generic;
using UnityEngine;

namespace LiteFramework.Extend
{
    public class Debugger : MonoBehaviour
    {
        private struct Log
        {
            public string Message_;
            public string StackTrace_;
            public LogType Type_;
        }

        private static readonly Dictionary<LogType, Color> LogTypeColors_ = new Dictionary<LogType, Color>
        {
            {LogType.Assert, Color.white},
            {LogType.Error, Color.red},
            {LogType.Exception, Color.red},
            {LogType.Warning, Color.yellow},
            {LogType.Log, Color.white}
        };

        public bool Enabled = false;

        private const float ShakeAcceleration_ = 3.0f;
        private const string WindowTitle_ = "Console";
        private static readonly GUIContent ClearLabel_ = new GUIContent("Clear", "clear the contents of the console");
        private static readonly GUIContent HideLabel_ = new GUIContent("Hide", "hide the window");
        private static readonly GUIContent OnlyShowErrorLabel_ = new GUIContent("Error", "only show error msg");
        private static readonly GUIContent ShowAllLabel_ = new GUIContent("All", "show all msg");
        private static readonly GUIContent CollapseLabel_ = new GUIContent("Collapse", "hide repeated message");
        private static readonly GUIContent StackTraceLabel_ = new GUIContent("Trace", "show stack trace");
        private static readonly GUIStyle FontStyle_ = new GUIStyle();

        private const int Margin_ = 10;
        private Rect TitleBarRect_ = new Rect(0, 0, Screen.width, 20);

        private Rect WindowRect_ =
            new Rect(Margin_, Margin_, Screen.width - (Margin_ * 2), Screen.height - (Margin_ * 2));

        private readonly List<Log> Logs_ = new List<Log>();
        private bool Collapse_ = false;
        private bool ShowStackTrace_ = false;
        private bool OnlyShowError_ = false;
        private Vector2 ScrollPosition_ = Vector2.zero;

        private int NeedTouchCount_ = 3;
        private float TouchedTime_ = 0.0f;
        private float MaxTouchedTime_ = 2.0f;

        void Start()
        {
            Input.multiTouchEnabled = true;
            FontStyle_.fontSize = Screen.width / 40;
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F12))
            {
                ExchangeWindow();
            }
#endif
            if (Input.touchCount == NeedTouchCount_)
            {
                TouchedTime_ += Time.deltaTime;

                if (TouchedTime_ >= MaxTouchedTime_)
                {
                    TouchedTime_ = 0.0f;
                    ExchangeWindow();
                }
            }
            else
            {
                TouchedTime_ = 0.0f;
            }
        }

        void OnGUI()
        {
            if (!Enabled)
            {
                return;
            }

            WindowRect_ = GUILayout.Window(1, WindowRect_, DrawConsoleWindow, WindowTitle_);
        }

        private void ExchangeWindow()
        {
            Enabled = !Enabled;
            TitleBarRect_ = new Rect(0, 0, Screen.width, 20);
            WindowRect_ = new Rect(Margin_, Margin_, Screen.width - (Margin_ * 2), Screen.height - (Margin_ * 2));
        }

        private void DrawConsoleWindow(int WindowID)
        {
            DrawToolBar();
            DrawLogsList();

            GUI.DragWindow(TitleBarRect_);
        }

        private void DrawLogsList()
        {
            ScrollPosition_ = GUILayout.BeginScrollView(ScrollPosition_);

            for (var Index = 0; Index < Logs_.Count; ++Index)
            {
                if (OnlyShowError_)
                {
                    if (Logs_[Index].Type_ != LogType.Error)
                    {
                        continue;
                    }
                }

                if (Collapse_ && Index > 0)
                {
                    var PreviousMessage = Logs_[Index - 1].Message_;

                    if (Logs_[Index].Message_ == PreviousMessage)
                    {
                        continue;
                    }
                }

                GUI.contentColor = LogTypeColors_[Logs_[Index].Type_];
                FontStyle_.normal.textColor = LogTypeColors_[Logs_[Index].Type_];
                GUILayout.Label(Logs_[Index].Message_, FontStyle_);

                if (ShowStackTrace_)
                {
                    GUILayout.Label(Logs_[Index].StackTrace_, FontStyle_);
                }
            }

            GUILayout.EndScrollView();
            GUI.contentColor = Color.white;
        }

        private void DrawToolBar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(ClearLabel_, GUILayout.Width(Screen.width * 0.15f),
                GUILayout.Height(Screen.height * 0.05f)))
            {
                Logs_.Clear();
            }

            if (GUILayout.Button(HideLabel_, GUILayout.Width(Screen.width * 0.15f),
                GUILayout.Height(Screen.height * 0.05f)))
            {
                ExchangeWindow();
            }

            if (OnlyShowError_)
            {
                if (GUILayout.Button(ShowAllLabel_, GUILayout.Width(Screen.width * 0.15f),
                    GUILayout.Height(Screen.height * 0.05f)))
                {
                    OnlyShowError_ = false;
                }
            }
            else
            {
                if (GUILayout.Button(OnlyShowErrorLabel_, GUILayout.Width(Screen.width * 0.15f),
                    GUILayout.Height(Screen.height * 0.05f)))
                {
                    OnlyShowError_ = true;
                }
            }

            if (GUILayout.Button(CollapseLabel_, GUILayout.Width(Screen.width * 0.15f),
                GUILayout.Height(Screen.height * 0.05f)))
            {
                Collapse_ = !Collapse_;
            }

            if (GUILayout.Button(StackTraceLabel_, GUILayout.Width(Screen.width * 0.15f),
                GUILayout.Height(Screen.height * 0.05f)))
            {
                ShowStackTrace_ = !ShowStackTrace_;
            }

            /*Collapse_ = GUILayout.Toggle(Collapse_, CollapseLabel_, GUILayout.Width(Screen.width * 0.15f),
                GUILayout.Height(Screen.height * 0.05f));
            ShowStackTrace_ = GUILayout.Toggle(ShowStackTrace_, StackTraceLabel_,
                GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.05f));*/
            GUILayout.EndHorizontal();
        }

        private void HandleLog(string Message, string StackTrace, LogType Type)
        {
            Logs_.Add(new Log
            {
                Message_ = Message,
                StackTrace_ = StackTrace,
                Type_ = Type
            });

            if (Type == LogType.Error)
            {
                if (!Enabled)
                {
                    OnlyShowError_ = true;
                    ShowStackTrace_ = true;
                    ExchangeWindow();
                }
            }
        }
    }
}