using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Core.ObjectPool
{
    public class GameObjectPool : ObjectPoolEntity<GameObject>
    {
        private readonly GameObject Prefab_;

        public GameObjectPool(string PoolName, GameObject Prefab)
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

        private GameObject OnCreate()
        {
            var Obj = Prefab_ == null ? new GameObject() : Object.Instantiate(Prefab_);
            Obj.transform.localPosition = Vector3.zero;
            Obj.transform.localRotation = Quaternion.identity;
            Obj.transform.localScale = Vector3.one;
            return Obj;
        }

        private void OnSpawn(GameObject Entity)
        {
            Entity.SetActive(true);
        }

        private void OnRecycle(GameObject Entity)
        {
            Entity.SetActive(false);
        }

        private void OnDispose(GameObject Entity)
        {
            Object.Destroy(Entity);
        }
    }
}