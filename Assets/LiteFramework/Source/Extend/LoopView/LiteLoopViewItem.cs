using UnityEngine;

namespace LiteFramework.Extend.LoopView
{
    public class LiteLoopViewItem
    {
        public GameObject Obj { get; }
        public object Param { get; }

        public LiteLoopViewItem(GameObject Obj, object Param)
        {
            this.Obj = Obj;
            this.Param = Param;
        }
    }
}