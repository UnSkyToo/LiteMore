using System.Collections.Generic;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public static class BulletManager
    {
        private static readonly Dictionary<BulletType, string> BulletPrefabName_ = new Dictionary<BulletType, string>();
        private static readonly List<BaseBullet> BulletList_ = new List<BaseBullet>();

        public static bool Startup()
        {
            BulletPrefabName_.Clear();
            BulletPrefabName_.Add(BulletType.Track, "Prefabs/Bullet/Bullet.prefab".ToLower());
            BulletPrefabName_.Add(BulletType.Laser, "Prefabs/Bullet/BulletLaser.prefab".ToLower());
            BulletPrefabName_.Add(BulletType.Bomb, "Prefabs/Bullet/BulletBomb.prefab".ToLower());
            BulletPrefabName_.Add(BulletType.Back, "Prefabs/Bullet/BulletBack.prefab".ToLower());
            BulletPrefabName_.Add(BulletType.Trigger, "Prefabs/Bullet/BulletTrigger.prefab".ToLower());

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
            BulletPrefabName_.Clear();
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

        private static GameObject CreateBullet(BulletType Type, Vector2 Position)
        {
            var Obj = AssetManager.CreatePrefabSync(BulletPrefabName_[Type]);
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