namespace LiteFramework.Core.Event
{
    public sealed class EnterForegroundEvent : BaseEvent
    {
        public EnterForegroundEvent()
            : base()
        {
        }
    }

    public sealed class EnterBackgroundEvent : BaseEvent
    {
        public EnterBackgroundEvent()
            : base()
        {
        }
    }

    public sealed class OpenUIEvent : BaseEvent
    {
        public OpenUIEvent()
        {
        }
    }

    public sealed class CloseUIEvent : BaseEvent
    {
        public CloseUIEvent()
        {
        }
    }
}