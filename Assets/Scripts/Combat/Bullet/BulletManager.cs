using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public static class BulletManager
    {
        private static Transform BulletRoot_;
        private static readonly Dictionary<BulletType, GameObject> BulletPrefab_ = new Dictionary<BulletType, GameObject>();
        private static readonly List<BulletBase> BulletList_ = new List<BulletBase>();

        public static bool Startup()
        {
            BulletRoot_ = GameObject.Find("Bullet").transform;

            BulletPrefab_.Clear();
            BulletPrefab_.Add(BulletType.Track, Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            BulletPrefab_.Add(BulletType.Laser, Resources.Load<GameObject>("Prefabs/Bullet/BulletLaser"));
            BulletPrefab_.Add(BulletType.Bomb, Resources.Load<GameObject>("Prefabs/Bullet/BulletBomb"));

            foreach (var Checker in BulletPrefab_)
            {
                if (Checker.Value == null)
                {
                    Debug.LogError($"BulletManager : null model prefab of {Checker.Key}");
                    return false;
                }
            }

            BulletList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in BulletList_)
            {
                Entity.Destroy();
            }

            BulletList_.Clear();
            BulletPrefab_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = BulletList_.Count - 1; Index >= 0; --Index)
            {
                BulletList_[Index].Tick(DeltaTime);

                if (!BulletList_[Index].IsAlive)
                {
                    BulletList_[Index].Destroy();
                    BulletList_.RemoveAt(Index);
                }
            }
        }

        private static GameObject CreateBullet(BulletType Type, Vector2 Position)
        {
            var Obj = Object.Instantiate(BulletPrefab_[Type]);
            Obj.transform.SetParent(BulletRoot_, false);
            Obj.transform.localPosition = Position;
            return Obj;
        }

        public static TrackBullet AddTrackBullet(string ResName, Vector2 Position)
        {
            var Obj = CreateBullet(BulletType.Track, Position);

            var Entity = new TrackBullet(Obj.transform, ResName);
            Entity.Create();
            BulletList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static LaserBullet AddLaserBullet(Vector2 Position)
        {
            var Obj = CreateBullet(BulletType.Laser, Position);

            var Entity = new LaserBullet(Obj.transform);
            Entity.Create();
            BulletList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static BombBullet AddBombBullet(Vector2 TargetPosition)
        {
            var Obj = CreateBullet(BulletType.Bomb, new Vector2(TargetPosition.x, 400));

            var Entity = new BombBullet(Obj.transform, TargetPosition);
            Entity.Create();
            BulletList_.Add(Entity);
            Entity.Position = Obj.transform.localPosition;

            return Entity;
        }

        public static int GetCount()
        {
            return BulletList_.Count;
        }
    }
}