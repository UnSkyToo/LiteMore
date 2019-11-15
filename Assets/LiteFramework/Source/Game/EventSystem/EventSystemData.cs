using UnityEngine;
using UnityEngine.EventSystems;

namespace LiteFramework.Game.EventSystem
{
    public class EventSystemData
    {
        public int ID { get; }
        public GameObject Sender { get; }
        public Vector2 Location { get; }
        public Vector2 Delta { get; }

        public EventSystemData(int ID, GameObject Sender, Vector2 Location, Vector2 Delta)
        {
            this.ID = ID;
            this.Sender = Sender;
            this.Location = Location;
            this.Delta = Delta;
        }

        public EventSystemData(PointerEventData Data)
        {
            this.ID = Data.pointerId;
            this.Sender = Data.pointerPress;
            this.Location = Data.position;
            this.Delta = Data.delta;
        }
    }
}