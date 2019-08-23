using LiteFramework.Game.Asset;
using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat
{
    public static class MapManager
    {
        private static Transform MapRoot_;
        private static Transform TileRoot_;
        private static Transform GroundRoot_;
        private static Transform NpcRoot_;
        private static Transform SkyRoot_;
        private static Transform TopRoot_;

        public static bool Startup()
        {
            MapRoot_ = new GameObject("Map").transform;
            MapRoot_.SetParent(Configure.CanvasRoot, false);

            TileRoot_ = MiscHelper.CreateCanvasLayer(MapRoot_, "TileRoot", Configure.TileOrder);
            GroundRoot_ = MiscHelper.CreateCanvasLayer(MapRoot_, "GroundRoot", Configure.GroundOrder);
            NpcRoot_ = MiscHelper.CreateCanvasLayer(MapRoot_, "NpcRoot", Configure.NpcOrder);
            SkyRoot_ = MiscHelper.CreateCanvasLayer(MapRoot_, "SkyRoot", Configure.SkyOrder);
            TopRoot_ = MiscHelper.CreateCanvasLayer(MapRoot_, "TopRoot", Configure.TopOrder);

            var Tex = AssetManager.CreateAssetSync<Sprite>("textures/m.png");
            for (var Y = -2; Y <= 2; ++Y)
            {
                for (var X = -3; X <= 3; ++X)
                {
                    var Obj = new GameObject($"m({X},{Y})");
                    Obj.layer = LayerMask.NameToLayer("UI");
                    Obj.AddComponent<SpriteRenderer>().sprite = Tex;
                    Obj.transform.SetParent(TileRoot_, false);
                    Obj.transform.localPosition = new Vector3(X * Tex.texture.width, Y * Tex.texture.height, 0);
                }
            }

            return true;
        }

        public static void Shutdown()
        {
            Object.Destroy(MapRoot_);

            /*var Map = GameObject.Find("Map");
            for (var Index = Map.transform.childCount - 1; Index >=0; --Index)
            {
                AssetManager.DeleteAsset(Map.transform.GetChild(Index).gameObject);
            }*/
        }

        public static void Tick(float DeltaTime)
        {
        }

        public static Transform AddToGroundLayer(Transform Entity)
        {
            if (Entity == null)
            {
                return null;
            }

            Entity.SetParent(GroundRoot_, false);
            Entity.GetComponent<SpriteRenderer>().sortingOrder = Configure.GroundOrder;
            return Entity;
        }

        public static Transform AddToNpcLayer(Transform Entity)
        {
            if (Entity == null)
            {
                return null;
            }

            Entity.SetParent(NpcRoot_, false);
            Entity.GetComponent<SpriteRenderer>().sortingOrder = Configure.NpcOrder;
            return Entity;
        }

        public static Transform AddToSkyLayer(Transform Entity)
        {
            if (Entity == null)
            {
                return null;
            }

            Entity.SetParent(SkyRoot_, false);
            Entity.GetComponent<SpriteRenderer>().sortingOrder = Configure.SkyOrder;
            return Entity;
        }

        public static Transform AddToTopLayer(Transform Entity)
        {
            if (Entity == null)
            {
                return null;
            }

            Entity.SetParent(SkyRoot_, false);

            var SR = Entity.GetComponent<SpriteRenderer>();
            if (SR != null)
            {
                SR.sortingOrder = Configure.TopOrder;
            }

            return Entity;
        }
    }
}