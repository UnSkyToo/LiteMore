using UnityEngine.EventSystems;

namespace LiteFramework.Game.EventSystem
{
    public class EventSystemCancelHandler : EventSystemBaseHandler, ICancelHandler
    {
        public void OnCancel(BaseEventData EventData)
        {
            EventCallback_?.Invoke(null);
            EventCallbackEx_?.Invoke();
        }
    }
}