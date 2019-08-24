using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Buff
{
    public class TriggerBuffDescriptor : BaseBuffDescriptor
    {
        public NpcAttrModifyInfo Modify { get; }
        //public BaseShape Shape { get; }
        public float Radius { get; } // only support circle shape
        public int MaxTriggerCount { get; }

        public TriggerBuffDescriptor(string Name, float Duration, float Interval, float WaitTime, bool IsRefund, NpcAttrModifyInfo Modify, float Radius, int MaxTriggerCount)
            : base(Name, Duration, Interval, WaitTime, IsRefund)
        {
            this.Modify = Modify;
            this.Radius = Radius;
            this.MaxTriggerCount = MaxTriggerCount;
        }
    }

    public class TriggerBuff : BaseBuff
    {
        private Vector2 Position_;

        public Vector2 Position
        {
            get => Position_;
            set
            {
                Position_ = value;
                if (Obj_ != null)
                {
                    Obj_.localPosition = value;
                }
            }
        }

        public Quaternion Rotation { get; set; }

        private readonly Dictionary<BaseNpc, int> NpcList_;
        private readonly List<BaseNpc> NpcDieList_;
        private readonly CircleShape Shape_;
        private readonly NpcAttrModifyInfo Modify_;
        private readonly CombatTeam Team_;
        protected readonly int MaxTriggerCount_;

        private Transform Obj_;

        public TriggerBuff(TriggerBuffDescriptor Desc, Vector2 Pos, CombatTeam Team)
            : base(BuffType.Trigger, Desc)
        {
            NpcList_ = new Dictionary<BaseNpc, int>();
            NpcDieList_ = new List<BaseNpc>();
            Shape_ = new CircleShape(Desc.Radius);
            Modify_ = Desc.Modify;
            MaxTriggerCount_ = Desc.MaxTriggerCount;
            Team_ = Team;
            Position_ = Pos;
        }

        protected override void OnAttach()
        {
            // temp sfx
            Obj_ = AssetManager.CreatePrefabSync("prefabs/bv0.prefab").transform;
            MapManager.AddToGroundLayer(Obj_);
            Obj_.localPosition = Position;
            var SR = Obj_.GetComponent<SpriteRenderer>();
            SR.color = new Color(90.0f / 255.0f, 220.0f / 255.0f, 1.0f);
            SR.size = new Vector2(Shape_.Radius * 2, Shape_.Radius * 2);
        }

        protected override void OnDetach()
        {
            AssetManager.DeleteAsset(Obj_.gameObject);
        }

        protected override void OnTrigger()
        {
            foreach (var Entity in NpcList_)
            {
                if (!Entity.Key.IsValid())
                {
                    NpcDieList_.Add(Entity.Key);
                }
            }

            if (NpcDieList_.Count > 0)
            {
                foreach (var Entity in NpcDieList_)
                {
                    OnNpcExit(Entity);
                    NpcList_.Remove(Entity);
                }
                NpcDieList_.Clear();
            }

            var NpcList = NpcManager.GetNpcList(Team_);
            foreach (var Entity in NpcList)
            {
                if (IsAlive && Shape_.Contains(Position, Entity.Position, Rotation))
                {
                    if (!NpcList_.ContainsKey(Entity))
                    {
                        NpcList_.Add(Entity, 0);
                        OnNpcEnter(Entity);
                    }
                    else
                    {
                        OnNpcStay(Entity);
                    }
                }
                else
                {
                    if (NpcList_.ContainsKey(Entity))
                    {
                        OnNpcExit(Entity);
                        NpcList_.Remove(Entity);
                    }
                }
            }
        }

        private void OnNpcEnter(BaseNpc Target)
        {
            if (MaxTriggerCount_ > 0 && NpcList_[Target] >= MaxTriggerCount_)
            {
                return;
            }

            NpcList_[Target]++;
            Target.Data.Attr.ApplyModify(Modify_);
        }

        private void OnNpcStay(BaseNpc Target)
        {
            if (MaxTriggerCount_ > 0 && NpcList_[Target] >= MaxTriggerCount_)
            {
                return;
            }

            NpcList_[Target]++;
            Target.Data.Attr.ApplyModify(Modify_);
        }

        private void OnNpcExit(BaseNpc Target)
        {
            if (!IsRefund_)
            {
                return;
            }

            for (var Index = 0; Index < NpcList_[Target]; ++Index)
            {
                Target.Data.Attr.RestoreModify(Modify_);
            }
        }
    }
}