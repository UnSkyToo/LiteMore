using System.Collections.Generic;
using LiteFramework.Core.ObjectPool;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public static class BulletManager
    {
        private static readonly Dictionary<BulletType, GameObjectPool> BulletPool_ = new Dictionary<BulletType, GameObjectPool>();
        private static readonly List<BaseBullet> BulletList_ = new List<BaseBullet>();

        public static bool Startup()
        {
            BulletPool_.Add(BulletType.Track, ObjectPoolManager.CreateGameObjectPool("TrackBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/Bullet.prefab")));
            BulletPool_.Add(BulletType.Laser, ObjectPoolManager.CreateGameObjectPool("LaserBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletLaser.prefab")));
            BulletPool_.Add(BulletType.Bomb, ObjectPoolManager.CreateGameObjectPool("BombBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletBomb.prefab")));
            BulletPool_.Add(BulletType.Back, ObjectPoolManager.CreateGameObjectPool("BackBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletBack.prefab")));
            BulletPool_.Add(BulletType.Trigger, ObjectPoolManager.CreateGameObjectPool("TriggerBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletTrigger.prefab")));

            BulletList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in BulletList_)
            {
                Entity.Dispose();
            }
            BulletList_.Clear();

            foreach (var Pool in BulletPool_)
            {
                ObjectPoolManager.DeletePool(Pool.Value);
            }
            BulletPool_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = BulletList_.Count - 1; Index >= 0; --Index)
            {
                BulletList_[Index].Tick(DeltaTime);

                if (!BulletList_[Index].IsAlive)
                {
                    BulletList_[Index].Dispose();
                    BulletList_.RemoveAt(Index);
                }
            }
        }

        public static void DisposeBullet(BaseBullet Bullet)
        {
            Bullet.IsAlive = false;
            BulletPool_[Bullet.Type].Recycle(Bullet.Entity.gameObject);
        }

        private static GameObject CreateBullet(BulletType Type, Vector2 Position)
        {
            var Obj = BulletPool_[Type].Spawn();
            Obj.transform.SetParent(Configure.BulletRoot, false);
            Obj.transform.localPosition = Position;
            return Obj;
        }

        public static TrackBullet AddTrackBullet(TrackBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Track, Desc.BaseBulletDesc.Position);

            var Entity = new TrackBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static LaserBullet AddLaserBullet(LaserBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Laser, Desc.BaseBulletDesc.Position);

            var Entity = new LaserBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static BombBullet AddBombBullet(BombBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Bomb, Desc.BaseBulletDesc.Position);

            var Entity = new BombBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static BackBullet AddBackBullet(BackBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Back, Desc.BaseBulletDesc.Position);

            var Entity = new BackBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static AttrTriggerBullet AddAttrTriggerBullet(AttrTriggerBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Trigger, Desc.BaseTriggerDesc.BaseBulletDesc.Position);

            var Entity = new AttrTriggerBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static int GetCount()
        {
            return BulletList_.Count;
        }
    }
}