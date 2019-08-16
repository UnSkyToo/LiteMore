namespace LiteFramework.Core.Log
{
    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Fatal = 3
    }

    public class LogEvent
    {
        public LogLevel Level { get; }
        public string Msg { get; }

        public LogEvent(LogLevel Level, string Msg)
        {
            this.Level = Level;
            this.Msg = Msg;
        }
    }
}