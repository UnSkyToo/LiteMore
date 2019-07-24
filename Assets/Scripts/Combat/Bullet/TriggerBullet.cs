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
        protected readonly List<BaseNpc> NpcList_;

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

            NpcList_ = new List<BaseNpc>();
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
            for (var Index = NpcList_.Count - 1; Index >= 0; --Index)
            {
                if (!NpcList_[Index].IsAlive)
                {
                    OnNpcExit(NpcList_[Index]);
                    OnNpcDie(NpcList_[Index]);
                    NpcList_.RemoveAt(Index);
                }
            }

            foreach (var Entity in NpcManager.GetNpcList(Team.Opposite()))
            {
                if (!Entity.CanLocked())
                {
                    continue;
                }

                if (IsAlive && Shape_.Contains(Entity.Position))
                {
                    if (!NpcList_.Contains(Entity))
                    {
                        NpcList_.Add(Entity);
                        OnNpcEnter(Entity);
                    }
                    else
                    {
                        OnNpcStay(Entity);
                    }
                }
                else
                {
                    if (NpcList_.Contains(Entity))
                    {
                        NpcList_.Remove(Entity);
                        OnNpcExit(Entity);
                    }
                }
            }
        }

        protected abstract void OnNpcEnter(BaseNpc Target);
        protected abstract void OnNpcStay(BaseNpc Target);
        protected abstract void OnNpcExit(BaseNpc Target);
        protected abstract void OnNpcDie(BaseNpc Target);
    }

    public class DamageTriggerBullet : BaseTriggerBullet
    {
        public DamageTriggerBullet(Transform Trans, BaseTriggerBulletDescriptor Desc)
            : base(Trans, Desc)
        {
        }

        protected override void OnNpcEnter(BaseNpc Target)
        {
            Target.OnBulletHit(this);
            LabelManager.AddNumberLabel(Target.Position, NumberLabelType.Bomb, Damage);
        }

        protected override void OnNpcStay(BaseNpc Target)
        {
        }

        protected override void OnNpcExit(BaseNpc Target)
        {
        }

        protected override void OnNpcDie(BaseNpc Target)
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
        private int ApplyCount_;

        public AttrTriggerBullet(Transform Trans, AttrTriggerBulletDescriptor Desc)
            : base(Trans, Desc.BaseTriggerDesc)
        {
            ModifyList_ = Desc.ModifyList;
            ApplyCount_ = 0;
        }

        protected override void OnNpcEnter(BaseNpc Target)
        {
            foreach (var Modify in ModifyList_)
            {
                Target.Attr.ApplyModify(Modify);
            }

            ApplyCount_++;
        }

        protected override void OnNpcStay(BaseNpc Target)
        {
        }

        protected override void OnNpcExit(BaseNpc Target)
        {
            for (var Index = 0; Index < ApplyCount_; ++Index)
            {
                foreach (var Modify in ModifyList_)
                {
                    Target.Attr.RestoreModify(Modify);
                }
            }
        }

        protected override void OnNpcDie(BaseNpc Target)
        {
        }
    }
}