using UnityEngine;

namespace LiteFramework.Core.Log
{
    public interface ILogAppender
    {
        void Append(LogEvent Event);
        void SetFormatter(ILogFormatter Formatter);
    }

    public abstract class LogAppenderBase : ILogAppender
    {
        public string Name { get; }
        public ILogFormatter LogFormatter { get; protected set; }

        protected LogAppenderBase(string Name, ILogFormatter Formatter)
        {
            this.Name = Name;
            this.LogFormatter = Formatter;
        }

        public abstract void Append(LogEvent Event);

        public void SetFormatter(ILogFormatter Formatter)
        {
            this.LogFormatter = Formatter;
        }
    }

    public class LogAppenderEmpty : LogAppenderBase
    {
        public LogAppenderEmpty(ILogFormatter Formatter)
            : base("Empty", Formatter)
        {
        }

        public override void Append(LogEvent Event)
        {
        }
    }

    public class LogAppenderConsole : LogAppenderBase
    {
        public LogAppenderConsole(ILogFormatter Formatter)
            : base("Console", Formatter)
        {
        }

        public override void Append(LogEvent Event)
        {
            switch (Event.Level)
            {
                case LogLevel.Info:
                    Debug.Log(LogFormatter.Format(Event));
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(LogFormatter.Format(Event));
                    break;
                case LogLevel.Error:
                    Debug.LogError(LogFormatter.Format(Event));
                    break;
                case LogLevel.Fatal:
                    Debug.LogAssertion(LogFormatter.Format(Event));
                    break;
                default:
                    break;
            }
        }
    }

    public class LogAppenderUnityEditor : LogAppenderBase
    {
        public LogAppenderUnityEditor(ILogFormatter Formatter)
            : base("UnityEditor", Formatter)
        {
        }

        public override void Append(LogEvent Event)
        {
            switch (Event.Level)
            {
                case LogLevel.Error:
                case LogLevel.Fatal:
#if UNITY_EDITOR
                    LiteFramework.Helper.UnityHelper.ShowEditorNotification(LogFormatter.Format(Event));
#endif
                    break;
                default:
                    break;
            }
        }
    }
}