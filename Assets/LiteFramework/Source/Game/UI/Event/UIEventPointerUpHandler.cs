using UnityEngine.EventSystems;

namespace LiteFramework.Game.UI.Event
{
    public class UIEventPointerUpHandler : UIEventBaseHandler, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData EventData)
        {
            // 穿透问题
            if (EventData.rawPointerPress != null && gameObject != EventData.rawPointerPress)
            {
                return;
            }

            EventCallback_?.Invoke(EventData.pointerPress, EventData.position);
            EventCallbackEx_?.Invoke();
        }
    }
}