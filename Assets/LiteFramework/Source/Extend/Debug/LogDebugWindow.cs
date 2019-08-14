using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class LogDebugWindow : BaseDebugWindow
    {
        private class LogInfo
        {
            public string Message;
            public string StackTrace;
            public LogType Type;
        }

        private readonly List<LogInfo> Logs_ = new List<LogInfo>();
        private Vector2 ScrollPosition_ = Vector2.zero;
        private Vector2 TraceScrollPosition_ = Vector2.zero;
        private uint InfoCount_ = 0;
        private bool IsShowInfo_ = true;
        private uint WarningCount_ = 0;
        private bool IsShowWarning_ = true;
        private uint ErrorCount_ = 0;
        private bool IsShowError_ = true;
        private LogInfo TraceNode_ = null;

        private readonly TextEditor TextEditor_ = new TextEditor();

        internal LogDebugWindow()
            : base()
        {
        }

        internal override bool Startup()
        {
            Logs_.Clear();
            ScrollPosition_ = Vector2.zero;
            TraceScrollPosition_ = Vector2.zero;

            InfoCount_ = 0;
            IsShowInfo_ = true;
            WarningCount_ = 0;
            IsShowWarning_ = true;
            ErrorCount_ = 0;
            IsShowError_ = true;
            TraceNode_ = null;

            Application.logMessageReceived += HandleLog;
            return true;
        }

        internal override void Shutdown()
        {
            Application.logMessageReceived -= HandleLog;
        }

        internal override void Tick(float DeltaTime)
        {
        }

        internal override void Draw()
        {
            DrawToolBar();
            DrawLogsList();
        }

        private void HandleLog(string Message, string StackTrace, LogType Type)
        {
            Logs_.Add(new LogInfo
            {
                Message = Message,
                StackTrace = StackTrace,
                Type = Type
            });
        }

        private void DrawToolBar()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<b>Clear</b>", GUILayout.Width(100)))
            {
                Logs_.Clear();
            }

            GUILayout.FlexibleSpace();

            IsShowInfo_ = GUILayout.Toggle(IsShowInfo_, $"Info({InfoCount_})", GUILayout.Width(90));
            IsShowWarning_ = GUILayout.Toggle(IsShowWarning_, $"Warning({WarningCount_})", GUILayout.Width(90));
            IsShowError_ = GUILayout.Toggle(IsShowError_, $"Error({ErrorCount_})", GUILayout.Width(90));

            GUILayout.EndHorizontal();
        }

        private string GetLogMsg(LogInfo Log)
        {
            switch (Log.Type)
            {
                case LogType.Assert:
                    return $"<color=White>{Log.Message}</color>";
                case LogType.Error:
                    return $"<color=Red>{Log.Message}</color>";
                case LogType.Exception:
                    return $"<color=Red>{Log.Message}</color>";
                case LogType.Warning:
                    return $"<color=Yellow>{Log.Message}</color>";
                case LogType.Log:
                    return $"<color=White>{Log.Message}</color>";
            }

            return string.Empty;
        }

        private void DrawLogsList()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            {
                ScrollPosition_ = GUILayout.BeginScrollView(ScrollPosition_);
                {
                    var IsSelected = false;
                    InfoCount_ = 0;
                    WarningCount_ = 0;
                    ErrorCount_ = 0;
                    foreach (var Log in Logs_)
                    {
                        switch (Log.Type)
                        {
                            case LogType.Log:
                                InfoCount_++;
                                if (!IsShowInfo_)
                                {
                                    continue;
                                }

                                break;
                            case LogType.Warning:
                                WarningCount_++;
                                if (!IsShowWarning_)
                                {
                                    continue;
                                }

                                break;
                            case LogType.Error:
                                ErrorCount_++;
                                if (!IsShowError_)
                                {
                                    continue;
                                }

                                break;
                            default:
                                break;
                        }

                        if (GUILayout.Toggle(TraceNode_ == Log, GetLogMsg(Log)))
                        {
                            IsSelected = true;
                            if (TraceNode_ != Log)
                            {
                                TraceNode_ = Log;
                                TraceScrollPosition_ = Vector2.zero;
                            }
                        }
                    }

                    if (!IsSelected)
                    {
                        TraceNode_ = null;
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            {
                TraceScrollPosition_ = GUILayout.BeginScrollView(TraceScrollPosition_, GUILayout.Height(100f));
                {
                    if (TraceNode_ != null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"<b>{GetLogMsg(TraceNode_)}</b>");
                        if (GUILayout.Button("Copy", GUILayout.Width(60f), GUILayout.Height(30f)))
                        {
                            TextEditor_.text = $"{TraceNode_.Message}{Environment.NewLine}{TraceNode_.StackTrace}{Environment.NewLine}";
                            TextEditor_.OnFocus();
                            TextEditor_.Copy();
                            TextEditor_.text = null;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Label(TraceNode_.StackTrace);
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
    }
}