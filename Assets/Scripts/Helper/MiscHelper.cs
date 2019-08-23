using UnityEngine;

namespace LiteMore.Helper
{
    public static class MiscHelper
    {
        public static Transform CreateCanvasLayer(Transform Parent, string Name, int Order)
        {
            var Obj = new GameObject(Name);
            Obj.transform.SetParent(Parent, false);
            Obj.AddComponent<Canvas>().overrideSorting = true;
            Obj.GetComponent<Canvas>().sortingOrder = Order;
            return Obj.transform;
        }
    }
}