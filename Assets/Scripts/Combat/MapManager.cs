﻿using UnityEngine;

namespace LiteMore.Combat
{
    public static class MapManager
    {
        public static Vector2 BuildPosition { get; private set; }

        public static bool Startup()
        {
            var Map = GameObject.Find("Map");
            var Tex = Resources.Load<Sprite>("Textures/m");
            for (var Y = -2; Y <= 2; ++Y)
            {
                for (var X = -3; X <= 3; ++X)
                {
                    var Obj = new GameObject($"m({X},{Y})");
                    Obj.layer = LayerMask.NameToLayer("UI");
                    Obj.AddComponent<SpriteRenderer>().sprite = Tex;
                    Obj.GetComponent<SpriteRenderer>().sortingOrder = -10;
                    Obj.transform.SetParent(Map.transform, false);
                    Obj.transform.localPosition = new Vector3(X * Tex.texture.width, Y * Tex.texture.height, 0);
                }
            }

            BuildPosition = new Vector2(Configure.WindowRight - 262.0f / 2.0f, 233.0f / 2.0f - 20);

            var Build = new GameObject("build");
            Build.layer = LayerMask.NameToLayer("UI");
            Build.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/build");
            Build.GetComponent<SpriteRenderer>().sortingOrder = -5;
            Build.GetComponent<SpriteRenderer>().flipX = true;
            Build.transform.SetParent(Map.transform, false);
            Build.transform.localPosition = new Vector3(Configure.WindowRight - 262.0f / 2.0f, 0);

            return true;
        }

        public static void Shutdown()
        {
            var Map = GameObject.Find("Map");
            for (var Index = Map.transform.childCount - 1; Index >=0; --Index)
            {
                Object.Destroy(Map.transform.GetChild(Index).gameObject);
            }
        }

        public static void Tick(float DeltaTime)
        {
        }
    }
}