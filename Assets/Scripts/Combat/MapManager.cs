using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteMore.Combat
{
    public static class MapManager
    {
        public static bool Startup()
        {
            var Tex = AssetManager.CreateAssetSync<Sprite>("textures/m.png");
            for (var Y = -2; Y <= 2; ++Y)
            {
                for (var X = -3; X <= 3; ++X)
                {
                    var Obj = new GameObject($"m({X},{Y})");
                    Obj.layer = LayerMask.NameToLayer("UI");
                    Obj.AddComponent<SpriteRenderer>().sprite = Tex;
                    Obj.GetComponent<SpriteRenderer>().sortingOrder = -10;
                    Obj.transform.SetParent(Configure.MapRoot, false);
                    Obj.transform.localPosition = new Vector3(X * Tex.texture.width, Y * Tex.texture.height, 0);
                }
            }

            return true;
        }

        public static void Shutdown()
        {
            var Map = GameObject.Find("Map");
            for (var Index = Map.transform.childCount - 1; Index >=0; --Index)
            {
                AssetManager.DeleteAsset(Map.transform.GetChild(Index).gameObject);
            }
        }

        public static void Tick(float DeltaTime)
        {
        }
    }
}