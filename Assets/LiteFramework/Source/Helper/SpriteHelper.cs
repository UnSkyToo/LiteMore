using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class SpriteHelper
    {
        public static void ReplaceSprite(SpriteRenderer Master, AssetUri Uri)
        {
            if (Master == null || Uri == null)
            {
                return;
            }

            AssetManager.CreateAssetAsync<Sprite>(Uri, (Spr) =>
            {
                if (Spr == null)
                {
                    LLogger.LWarning($"can't load {Uri}");
                    return;
                }

                Master.sprite = Spr;
            });
        }

        public static void ChangeColor(Transform Parent, Color NewColor, bool Recursively)
        {
            var Graphics = Parent?.GetComponent<SpriteRenderer>();
            if (Graphics == null)
            {
                return;
            }

            Graphics.color = NewColor;

            if (!Recursively)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);
                ChangeColor(Child, NewColor, Recursively);
            }
        }
    }
}