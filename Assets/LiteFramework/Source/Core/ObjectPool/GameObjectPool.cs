using LiteFramework.Game.Asset;

namespace LiteFramework.Core.ObjectPool
{
    public class GameObjectPool : ObjectPoolEntity<UnityEngine.GameObject>
    {
        private readonly UnityEngine.GameObject Prefab_;

        public GameObjectPool(string PoolName, UnityEngine.GameObject Prefab)
            : base(PoolName, null, null, null, null)
        {
            this.Prefab_ = Prefab;
            this.CreateFunc_ = OnCreate;
            this.SpawnFunc_ = OnSpawn;
            this.RecycleFunc_ = OnRecycle;
            this.DisposeFunc_ = OnDispose;

            this.Prefab_.SetActive(false);
        }

        public override void Dispose()
        {
            base.Dispose();
            AssetManager.DeleteAsset(Prefab_);
        }

        private UnityEngine.GameObject OnCreate()
        {
            if (Prefab_ == null)
            {
                return new UnityEngine.GameObject();
            }
            else
            {
                return UnityEngine.Object.Instantiate(Prefab_);
            }
        }

        private void OnSpawn(UnityEngine.GameObject Entity)
        {
            Entity.SetActive(true);
        }

        private void OnRecycle(UnityEngine.GameObject Entity)
        {
            Entity.SetActive(false);
        }

        private void OnDispose(UnityEngine.GameObject Entity)
        {
            UnityEngine.Object.Destroy(Entity);
        }
    }
}