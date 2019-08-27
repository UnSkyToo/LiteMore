using UnityEngine;
using UnityEngine.EventSystems;

namespace LiteFramework.Game.UI.Event
{
    public class UIEventCancelHandler : UIEventBaseHandler, ICancelHandler
    {
        public void OnCancel(BaseEventData EventData)
        {
            EventCallback_?.Invoke(null, Vector2.zero);
            EventCallbackEx_?.Invoke();
        }
    }
}