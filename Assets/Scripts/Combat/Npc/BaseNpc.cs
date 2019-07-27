﻿using System.Collections.Generic;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Fsm;
using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class BaseNpc : GameEntity
    {
        public bool IsStatic { get; set; }
        public CombatTeam Team { get; protected set; }
        public NpcAttribute Attr { get; }
        public NpcDirection Direction { get; protected set; }
        public BaseNpc TargetNpc { get; set; }

        protected readonly NpcAnimation Animation_;
        protected readonly BaseFsm Fsm_;
        protected readonly NpcBar Bar_;

        protected Vector2 TargetPos_;
        private int HitSfxInterval_;
        protected List<BaseBullet> LockedList_;

        public BaseNpc(Transform Trans, CombatTeam Team, float[] InitAttr)
            : base(Trans)
        {
            IsStatic = false;
            this.Team = Team;

            Attr = new NpcAttribute(InitAttr);
            Attr.AttrChanged += OnAttrChanged;

            Direction = NpcDirection.None;
            TargetNpc = null;

            Animation_ = new NpcAnimation(Trans.GetComponent<Animator>());
            Animation_.RegisterMsg("Attack", 0.25f, CombatMsgCode.Atk, OnMsgCode);
            //Animation_.RegisterMsg("Attack", 0.25f, CombatMsgCode.Effect, OnMsgCode);

            Fsm_ = new BaseFsm(this);
            Fsm_.ChangeToIdleState();

            Bar_ = new NpcBar(this);
            Bar_.SetMaxHp(CalcFinalAttr(NpcAttrIndex.MaxHp));
            Bar_.SetMaxMp(CalcFinalAttr(NpcAttrIndex.MaxMp));

            TargetPos_ = Vector2.zero;
            HitSfxInterval_ = 0;
            LockedList_ = new List<BaseBullet>();

            SetDirection(NpcDirection.Right);
        }

        public override void Tick(float DeltaTime)
        {
            Animation_.Tick(DeltaTime);
            Fsm_.Tick(DeltaTime);
            Bar_.Tick(DeltaTime);
            UpdateAttr(DeltaTime);
            UpdateLockedList();
            HitSfxInterval_--;
            base.Tick(DeltaTime);
        }

        public void PlayAnimation(string AnimName, bool IsLoop)
        {
            Animation_.Play(AnimName, IsLoop);
        }

        public bool AnimationIsEnd()
        {
            return Animation_.IsEnd();
        }

        public bool IsFsmState(FsmStateName StateName)
        {
            return Fsm_.GetStateName() == StateName;
        }

        public void OnCombatEvent(CombatEvent Event)
        {
            Fsm_.OnCombatEvent(Event);
        }

        public void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            Fsm_.OnMsgCode(Animation, MsgCode);
        }

        public float CalcFinalAttr(NpcAttrIndex Index)
        {
            return Attr.CalcFinalValue(Index);
        }

        private void OnAttrChanged(NpcAttrIndex Index)
        {
            switch (Index)
            {
                case NpcAttrIndex.MaxHp:
                    Bar_.SetMaxHp(Attr.CalcFinalValue(NpcAttrIndex.MaxHp));
                    break;
                case NpcAttrIndex.MaxMp:
                    Bar_.SetMaxMp(Attr.CalcFinalValue(NpcAttrIndex.MaxMp));
                    break;
                case NpcAttrIndex.Speed:
                    MoveTo(TargetPos_);
                    break;
                default:
                    break;
            }
        }

        public void SetDirection(NpcDirection Dir)
        {
            if (Direction == Dir)
            {
                return;
            }

            Direction = Dir;
            switch (Direction)
            {
                case NpcDirection.Left:
                    GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case NpcDirection.Right:
                    GetComponent<SpriteRenderer>().flipX = false;
                    break;
                default:
                    break;
            }
        }

        public bool IsValidTarget()
        {
            return TargetNpc != null && TargetNpc.IsAlive;
        }

        private void UpdateAttr(float DeltaTime)
        {
            var Hp = Attr.CalcFinalValue(NpcAttrIndex.Hp) + Attr.CalcFinalValue(NpcAttrIndex.HpAdd) * DeltaTime;
            var MaxHp = Attr.CalcFinalValue(NpcAttrIndex.MaxHp);
            if (Hp > MaxHp)
            {
                Hp = MaxHp;
            }
            Attr.SetValue(NpcAttrIndex.Hp, Hp);

            var Mp = Attr.CalcFinalValue(NpcAttrIndex.Mp) + Attr.CalcFinalValue(NpcAttrIndex.MpAdd) * DeltaTime;
            var MaxMp = Attr.CalcFinalValue(NpcAttrIndex.MaxMp);
            if (Mp > MaxMp)
            {
                Mp = MaxMp;
            }
            Attr.SetValue(NpcAttrIndex.Mp, Mp);
        }

        private void UpdateLockedList()
        {
            for (var Index = LockedList_.Count - 1; Index >= 0; --Index)
            {
                if (!LockedList_[Index].IsAlive)
                {
                    LockedList_.RemoveAt(Index);
                }
            }
        }

        public void OnLocking(BaseBullet Attacker)
        {
            if (LockedList_.Contains(Attacker))
            {
                return;
            }

            LockedList_.Add(Attacker);
        }

        public float CalcForecastHp()
        {
            var Hp = CalcFinalAttr(NpcAttrIndex.Hp);
            foreach (var Attacker in LockedList_)
            {
                if (Attacker.IsAlive)
                {
                    Hp -= Attacker.Damage;
                }
            }
            return Hp;
        }

        public virtual bool CanLocked()
        {
            return CalcFinalAttr(NpcAttrIndex.Hp) > 0 && CalcForecastHp() > 0 && IsAlive;
        }

        public void OnBulletHit(BaseBullet Collider)
        {
            LockedList_.Remove(Collider);
            OnHitDamage(Collider.Damage);
        }

        public void OnNpcHit(BaseNpc Attacker)
        {
            OnHitDamage(Attacker.CalcFinalAttr(NpcAttrIndex.Damage));
        }

        private void OnHitDamage(float Damage)
        {
            if (IsFsmState(FsmStateName.Die))
            {
                return;
            }

            Attr.AddValue(NpcAttrIndex.Hp, -Damage);

            if (Attr.CalcFinalValue(NpcAttrIndex.Hp) <= 0)
            {
                Attr.SetValue(NpcAttrIndex.Hp, 0);
                PlayerManager.AddGem((int)CalcFinalAttr(NpcAttrIndex.Gem));
                SfxManager.AddSfx("Prefabs/Sfx/GoldSfx", Position);
                EventManager.Send(new NpcDieEvent(this));
            }

            TryToPlayHitSfx();
        }

        private void TryToPlayHitSfx()
        {
            if (HitSfxInterval_ <= 0)
            {
                SfxManager.AddSfx("Prefabs/Sfx/HitSfx", Position);
                HitSfxInterval_ = 8;
            }
        }

        public void MoveTo(Vector2 TargetPos)
        {
            if (IsStatic)
            {
                return;
            }

            TargetPos_ = TargetPos;
            EventManager.Send(new NpcWalkEvent(this, TargetPos));
        }

        public void SetDead()
        {
            IsAlive = false;
        }

        public void Back(float Distance, float Speed)
        {
            Back((Position - TargetPos_).normalized * Distance, Speed);
        }

        public void Back(Vector2 Offset, float Speed)
        {
            if (IsStatic)
            {
                return;
            }

            var BackPos = Position + Offset;
            EventManager.Send(new NpcBackEvent(this, BackPos, Offset.magnitude / Speed));
        }
    }
}