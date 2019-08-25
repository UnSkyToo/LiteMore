using System.Collections.Generic;
using LiteFramework.Core.Base;
using LiteFramework.Core.ObjectPool;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public static class BulletManager
    {
        private static readonly Dictionary<BulletType, GameObjectPool> BulletPool_ = new Dictionary<BulletType, GameObjectPool>();
        private static readonly ListEx<BaseBullet> BulletList_ = new ListEx<BaseBullet>();

        public static bool Startup()
        {
            BulletPool_.Add(BulletType.Track, ObjectPoolManager.CreateGameObjectPool("TrackBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/Bullet.prefab")));
            BulletPool_.Add(BulletType.Laser, ObjectPoolManager.CreateGameObjectPool("LaserBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletLaser.prefab")));
            BulletPool_.Add(BulletType.Bomb, ObjectPoolManager.CreateGameObjectPool("BombBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletBomb.prefab")));
            BulletPool_.Add(BulletType.Back, ObjectPoolManager.CreateGameObjectPool("BackBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletBack.prefab")));
            BulletPool_.Add(BulletType.Arrow, ObjectPoolManager.CreateGameObjectPool("ArrowBullet", AssetManager.CreatePrefabSync("Prefabs/Bullet/BulletArrow.prefab")));

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
            foreach (var Entity in BulletList_)
            {
                Entity.Tick(DeltaTime);

                if (!Entity.IsAlive)
                {
                    Entity.Dispose();
                    BulletList_.Remove(Entity);
                }
            }
            BulletList_.Flush();
        }

        public static void DisposeBullet(BaseBullet Bullet)
        {
            Bullet.IsAlive = false;
            BulletPool_[Bullet.Type].Recycle(Bullet.Entity.gameObject);
        }

        private static GameObject CreateBullet(BulletType Type, Vector2 Position)
        {
            var Obj = BulletPool_[Type].Spawn();
            MapManager.AddToSkyLayer(Obj.transform);
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

        public static ArrowBullet AddArrowBullet(ArrowBulletDescriptor Desc)
        {
            var Obj = CreateBullet(BulletType.Arrow, Desc.BaseBulletDesc.Position);

            var Entity = new ArrowBullet(Obj.transform, Desc);
            BulletList_.Add(Entity);

            return Entity;
        }

        public static int GetCount()
        {
            return BulletList_.Count;
        }
    }
}