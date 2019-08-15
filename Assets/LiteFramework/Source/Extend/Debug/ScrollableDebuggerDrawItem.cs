using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal abstract class ScrollableDebuggerDrawItem : BaseDebuggerDrawItem
    {
        private Vector2 ScrollPosition_ = Vector2.zero;

        public override void Draw()
        {
            ScrollPosition_ = GUILayout.BeginScrollView(ScrollPosition_);
            {
                OnDrawScrollable();
            }
            GUILayout.EndScrollView();
        }

        protected abstract void OnDrawScrollable();

        protected void DrawItem(string Title, string Content)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Title, GUILayout.Width(240));
                GUILayout.Label(Content);
            }
            GUILayout.EndHorizontal();
        }
    }
}