using LiteFramework.Core.Motion;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Game.Base
{
    /// <summary>
    /// UnityEngine Entity Binder
    /// </summary>
    public abstract class GameEntity : BaseEntity
    {
        public Vector2 Position
        {
            get => Transform_.localPosition;
            set => Transform_.localPosition = new Vector3(value.x, value.y, 1 - Mathf.Clamp(value.y / (float)Screen.height, -1, 1));
        }

        public Vector2 Scale
        {
            get => Transform_.localScale;
            set => Transform_.localScale = value;
        }

        public Quaternion Rotation
        {
            get => Transform_.localRotation;
            set => Transform_.localRotation = value;
        }

        private Transform Transform_;
        //public Transform Entity => Transform_;
        private readonly bool NotDeleteAsset_;
        private readonly bool KeepName_;
        private bool HasMotion_;

        protected GameEntity(string Name, Transform Trans, bool NotDeleteAsset, bool KeepName)
            : base(Name)
        {
            Transform_ = Trans;
            NotDeleteAsset_ = NotDeleteAsset;

            KeepName_ = KeepName;
            if (!KeepName_)
            {
                Transform_.name = $"{Name}<{ID}>";
            }

            //Position = Vector2.zero;
            //Scale = Vector2.one;
            Rotation = Quaternion.identity;

            HasMotion_ = false;
        }

        public override void Dispose()
        {
            IsAlive = false;
            if (Transform_ != null)
            {
                AbandonMotion();
                if (!NotDeleteAsset_)
                {
                    AssetManager.DeleteAsset(Transform_.gameObject);
                }
                Transform_ = null;
            }
        }

        public Transform GetTransform()
        {
            return Transform_;
        }

        public void SetName(string NewName)
        {
            Name = NewName;
            if (!KeepName_)
            {
                Transform_.name = NewName;
            }
        }

        public void SetActive(bool Value)
        {
            Transform_.gameObject.SetActive(Value);
        }

        public void SetActive(string ChildPath, bool Value)
        {
            var Obj = FindChild(ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(Value);
            }
        }

        public bool IsActive()
        {
            return Transform_.gameObject.activeSelf;
        }

        public bool IsActive(string ChildPath)
        {
            var Obj = FindChild(ChildPath);
            return Obj != null && Obj.gameObject.activeSelf;
        }

        public void EnableTouched(bool Value)
        {
            var Listener = GetComponent<UnityEngine.UI.Graphic>();
            if (Listener != null)
            {
                Listener.raycastTarget = Value;
            }
        }

        public void EnableTouched(string ChildPath, bool Value)
        {
            var Listener = GetComponent<UnityEngine.UI.Graphic>(ChildPath);
            if (Listener != null)
            {
                Listener.raycastTarget = Value;
            }
        }

        public void ExecuteMotion(BaseMotion Motion)
        {
            HasMotion_ = true;
            Transform_.ExecuteMotion(Motion);
        }

        public void AbandonMotion()
        {
            if (!HasMotion_)
            {
                return;
            }

            MotionManager.Abandon(Transform_);
            HasMotion_ = false;
        }

        public T AddComponent<T>() where T : Component
        {
            return Transform_.gameObject.AddComponent<T>();
        }

        public T GetComponent<T>()
        {
            return Transform_.GetComponent<T>();
        }

        public T GetComponent<T>(string Path)
        {
            var Child = Transform_?.Find(Path);
            return Child != null ? Child.GetComponent<T>() : default;
        }

        public Transform GetParent()
        {
            return Transform_.parent;
        }

        public void SetParent(Transform Parent, bool WorldPositionStays)
        {
            Transform_.SetParent(Parent, WorldPositionStays);
        }

        public Transform FindChild(string Path)
        {
            return Transform_.Find(Path);
        }
    }
}