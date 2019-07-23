using System.Collections.Generic;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public static class BulletManager
    {
        private static readonly Dictionary<BulletType, GameObject> BulletPrefab_ = new Dictionary<BulletType, GameObject>();
        private static readonly List<BaseBullet> BulletList_ = new List<BaseBullet>();

        public static bool Startup()
        {
            BulletPrefab_.Clear();
            BulletPrefab_.Add(BulletType.Track, Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            BulletPrefab_.Add(BulletType.Laser, Resources.Load<GameObject>("Prefabs/Bullet/BulletLaser"));
            BulletPrefab_.Add(BulletType.Bomb, Resources.Load<GameObject>("Prefabs/Bullet/BulletBomb"));
            BulletPrefab_.Add(BulletType.Back, Resources.Load<GameObject>("Prefabs/Bullet/BulletBack"));
            BulletPrefab_.Add(BulletType.Trigger, Resources.Load<GameObject>("Prefabs/Bullet/BulletTrigger"));

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
            Obj.transform.SetParent(Configure.BulletRoot, false);
            Obj.transform.localPosition = Position;
            return Obj;
        }

        public static TrackBullet AddTrackBullet(TrackBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Track, Desc.BaseBulletDesc.Position);

            var Entity = new TrackBullet(Obj.transform, Desc);
            Entity.Create();
            BulletList_.Add(Entity);

            return Entity;
        }

        public static LaserBullet AddLaserBullet(LaserBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Laser, Desc.BaseBulletDesc.Position);

            var Entity = new LaserBullet(Obj.transform, Desc);
            Entity.Create();
            BulletList_.Add(Entity);

            return Entity;
        }

        public static BombBullet AddBombBullet(BombBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Bomb, Desc.BaseBulletDesc.Position);

            var Entity = new BombBullet(Obj.transform, Desc);
            Entity.Create();
            BulletList_.Add(Entity);

            return Entity;
        }

        public static BackBullet AddBackBullet(BackBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Back, Desc.BaseBulletDesc.Position);

            var Entity = new BackBullet(Obj.transform, Desc);
            Entity.Create();
            BulletList_.Add(Entity);

            return Entity;
        }

        public static AttrTriggerBullet AddAttrTriggerBullet(AttrTriggerBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Trigger, Desc.BaseTriggerDesc.BaseBulletDesc.Position);

            var Entity = new AttrTriggerBullet(Obj.transform, Desc);
            Entity.Create();
            BulletList_.Add(Entity);

            return Entity;
        }

        public static int GetCount()
        {
            return BulletList_.Count;
        }
    }
}