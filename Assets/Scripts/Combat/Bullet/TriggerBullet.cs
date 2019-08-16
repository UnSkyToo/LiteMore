using System.Collections.Generic;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Bullet
{
    public struct BaseTriggerBulletDescriptor
    {
        public BaseBulletDescriptor BaseBulletDesc { get; }
        public float Radius { get; }
        public float Interval { get; }
        public int Count { get; }
        public Color CircleColor { get; }

        public BaseTriggerBulletDescriptor(BaseBulletDescriptor BaseBulletDesc, float Radius, float Interval, int Count, Color CircleColor)
        {
            this.BaseBulletDesc = BaseBulletDesc;
            this.Radius = Radius;
            this.Interval = Interval;
            this.Count = Count;
            this.CircleColor = CircleColor;
        }
    }

    public abstract class BaseTriggerBullet : BaseBullet
    {
        protected readonly CircleShape Shape_;
        protected readonly Dictionary<BaseNpc, uint> NpcList_;
        protected readonly List<BaseNpc> NpcExitList_;

        private readonly float Interval_;
        private float Time_;
        private int Count_;

        protected BaseTriggerBullet(Transform Trans, BaseTriggerBulletDescriptor Desc)
            : base(Trans, BulletType.Trigger, Desc.BaseBulletDesc)
        {
            Shape_ = new CircleShape(Position, Desc.Radius);

            var SR = Trans.GetComponent<SpriteRenderer>();
            SR.color = Desc.CircleColor;
            SR.size = new Vector2(Desc.Radius * 2, Desc.Radius * 2);

            Interval_ = Desc.Interval;
            Time_ = 0;
            Count_ = Desc.Count;

            NpcList_ = new Dictionary<BaseNpc, uint>();
            NpcExitList_ = new List<BaseNpc>();
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (!IsAlive)
            {
                return;
            }

            Time_ += DeltaTime;
            if (Time_ >= Interval_)
            {
                Time_ -= Interval_;

                if (Count_ > 0)
                {
                    Count_--;
                    IsAlive = (Count_ != 0);
                }

                Trigger();
            }
        }

        private void Trigger()
        {
            foreach (var Entity in NpcList_)
            {
                if (!Entity.Key.IsAlive)
                {
                    NpcExitList_.Add(Entity.Key);
                }
            }

            if (NpcExitList_.Count > 0)
            {
                foreach (var Entity in NpcExitList_)
                {
                    OnNpcExit(Entity, NpcList_[Entity]);
                    OnNpcDie(Entity, NpcList_[Entity]);
                    NpcList_.Remove(Entity);
                }
                NpcExitList_.Clear();
            }

            foreach (var Entity in NpcManager.GetNpcList(Team.Opposite()))
            {
                if (!Entity.CanLocked())
                {
                    continue;
                }

                if (IsAlive && Shape_.Contains(Entity.Position))
                {
                    if (!NpcList_.ContainsKey(Entity))
                    {
                        NpcList_.Add(Entity, 1);
                        OnNpcEnter(Entity, 1);
                    }
                    else
                    {
                        OnNpcStay(Entity, NpcList_[Entity]);
                    }
                }
                else
                {
                    if (NpcList_.ContainsKey(Entity))
                    {
                        OnNpcExit(Entity, NpcList_[Entity]);
                        NpcList_.Remove(Entity);
                    }
                }
            }
        }

        protected abstract void OnNpcEnter(BaseNpc Target, uint TotalApplyCount);
        protected abstract void OnNpcStay(BaseNpc Target, uint TotalApplyCount);
        protected abstract void OnNpcExit(BaseNpc Target, uint TotalApplyCount);
        protected abstract void OnNpcDie(BaseNpc Target, uint TotalApplyCount);
    }

    public class DamageTriggerBullet : BaseTriggerBullet
    {
        public DamageTriggerBullet(Transform Trans, BaseTriggerBulletDescriptor Desc)
            : base(Trans, Desc)
        {
        }

        protected override void OnNpcEnter(BaseNpc Target, uint TotalApplyCount)
        {
            Target.OnBulletHit(this);
            //LabelManager.AddNumberLabel(Target.Position, NumberLabelType.Bomb, Damage);
        }

        protected override void OnNpcStay(BaseNpc Target, uint TotalApplyCount)
        {
        }

        protected override void OnNpcExit(BaseNpc Target, uint TotalApplyCount)
        {
        }

        protected override void OnNpcDie(BaseNpc Target, uint TotalApplyCount)
        {
        }
    }

    public struct AttrTriggerBulletDescriptor
    {
        public BaseTriggerBulletDescriptor BaseTriggerDesc { get; }
        public List<NpcAttrModifyInfo> ModifyList { get; }

        public AttrTriggerBulletDescriptor(BaseTriggerBulletDescriptor BaseTriggerDesc, List<NpcAttrModifyInfo> ModifyList)
        {
            this.BaseTriggerDesc = BaseTriggerDesc;
            this.ModifyList = ModifyList;
        }
    }

    public class AttrTriggerBullet : BaseTriggerBullet
    {
        private readonly List<NpcAttrModifyInfo> ModifyList_;

        public AttrTriggerBullet(Transform Trans, AttrTriggerBulletDescriptor Desc)
            : base(Trans, Desc.BaseTriggerDesc)
        {
            ModifyList_ = Desc.ModifyList;
        }

        protected override void OnNpcEnter(BaseNpc Target, uint TotalApplyCount)
        {
            foreach (var Modify in ModifyList_)
            {
                Target.Attr.ApplyModify(Modify);
            }
        }

        protected override void OnNpcStay(BaseNpc Target, uint TotalApplyCount)
        {
        }

        protected override void OnNpcExit(BaseNpc Target, uint TotalApplyCount)
        {
            for (var Index = 0u; Index < TotalApplyCount; ++Index)
            {
                foreach (var Modify in ModifyList_)
                {
                    Target.Attr.RestoreModify(Modify);
                }
            }
        }

        protected override void OnNpcDie(BaseNpc Target, uint TotalApplyCount)
        {
        }
    }
}