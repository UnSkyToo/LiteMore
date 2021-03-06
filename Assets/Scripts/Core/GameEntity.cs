﻿using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteMore.Core
{
    /// <summary>
    /// UnityEngine Entity Binder
    /// </summary>
    public abstract class GameEntity : BaseEntity
    {
        private Vector2 Position_;
        public Vector2 Position
        {
            get => Position_;
            set
            {
                Position_ = value;
                Transform_.localPosition = new Vector3(value.x, value.y, 1 - Mathf.Clamp(value.y / (float)Screen.height, -1, 1));
            }
        }

        private Vector2 Scale_;
        public Vector2 Scale
        {
            get => Scale_;
            set
            {
                Scale_ = value;
                Transform_.localScale = value;
            }
        }

        private Quaternion Rotation_;
        public Quaternion Rotation
        {
            get => Rotation_;
            set
            {
                Rotation_ = value;
                Transform_.localRotation = value;
            }
        }

        protected Transform Transform_;
        public Transform Entity => Transform_;

        protected GameEntity(string Name, Transform Trans)
            : base(Name)
        {
            Transform_ = Trans;
            Transform_.name = $"<{ID}>";

            //Position = Vector2.zero;
            //Scale = Vector2.one;
            //Rotation = Quaternion.identity;
        }

        public override void Dispose()
        {
            IsAlive = false;
            if (Transform_ != null)
            {
                AssetManager.DeleteAsset(Transform_.gameObject);
                Transform_ = null;
            }
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
            return Transform_.Find(Path).GetComponent<T>();
        }
    }
}