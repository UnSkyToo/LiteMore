using System;

namespace LiteFramework.Core.Log
{
    public interface ILogFormatter
    {
        string Format(LogEvent Event);
    }

    public abstract class LogFormatterBase : ILogFormatter
    {
        public string Name { get; }

        protected LogFormatterBase(string Name)
        {
            this.Name = Name;
        }

        public abstract string Format(LogEvent Event);
    }

    public class LogFormatterNormal : LogFormatterBase
    {
        public LogFormatterNormal()
            : base("Normal")
        {
        }

        public override string Format(LogEvent Event)
        {
            var Msg = $"[{DateTime.Now.ToLongTimeString()}] - [{Event.Level.ToString()}] {Event.Msg}";
            return Msg;
        }
    }
}