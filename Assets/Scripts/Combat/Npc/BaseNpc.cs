using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Fsm;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc.Handler;
using LiteMore.Core;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc : GameEntity
    {
        public bool IsStatic { get; set; }
        public CombatTeam Team { get; protected set; }

        protected readonly NpcBar Bar_;
        protected List<BaseBullet> LockedList_;
        protected readonly List<BaseNpcHandler> HandlerList_;

        public BaseNpc(string Name, Transform Trans, CombatTeam Team, NpcAttribute InitAttr)
            : base(Name, Trans)
        {
            this.IsStatic = false;
            this.Team = Team;

            Attr = InitAttr;
            Attr.AttrChanged += OnAttrChanged;
            State_ = 0;

            Direction = NpcDirection.None;
            AttackerNpc = null;
            TargetNpc = null;

            Animation_ = new NpcAnimation(Trans.GetComponent<Animator>());
            Animation_.RegisterMsg("Attack", 0.25f, CombatMsgCode.Atk, OnMsgCode);
            //Animation_.RegisterMsg("Attack", 0.25f, CombatMsgCode.Effect, OnMsgCode);

            Fsm_ = new BaseFsm(this);
            Fsm_.ChangeToIdleState();

            Bar_ = new NpcBar(this);
            Bar_.SetMaxHp(CalcFinalAttr(NpcAttrIndex.MaxHp));
            Bar_.SetMaxMp(CalcFinalAttr(NpcAttrIndex.MaxMp));

            TargetPos = Vector2.zero;
            HitSfxInterval_ = 0;
            LockedList_ = new List<BaseBullet>();

            HandlerList_ = new List<BaseNpcHandler>();

            SetDirection(NpcDirection.Right);
        }

        public override void Tick(float DeltaTime)
        {
            foreach (var Handler in HandlerList_)
            {
                Handler.OnTick(DeltaTime);
            }

            Animation_.Tick(DeltaTime);
            Fsm_.Tick(DeltaTime);
            Bar_.Tick(DeltaTime);
            UpdateAttr(DeltaTime);
            UpdateLockedList();
            HitSfxInterval_--;
            base.Tick(DeltaTime);
        }

        public void OnCombatEvent(CombatEvent Event)
        {
            Fsm_.OnCombatEvent(Event);
        }

        public void OnMsgCode(string Animation, CombatMsgCode MsgCode)
        {
            Fsm_.OnMsgCode(Animation, MsgCode);
        }

        
        public void RegisterHandler(BaseNpcHandler Handler)
        {
            if (HandlerList_.Contains(Handler))
            {
                return;
            }

            Handler.Master = this;
            HandlerList_.Add(Handler);
            HandlerList_.Sort((A, B) =>
            {
                if (A.Priority > B.Priority)
                {
                    return -1;
                }

                if (A.Priority < B.Priority)
                {
                    return 1;
                }

                return 0;
            });
        }

        public void UnRegisterHandler(BaseNpcHandler Handler)
        {
            Handler.Master = null;
            HandlerList_.Remove(Handler);
        }

        public bool IsValid()
        {
            return CanLocked();
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
            return IsAlive && !IsFsmState(FsmStateName.Die) && CalcFinalAttr(NpcAttrIndex.Hp) > 0 && CalcForecastHp() > 0;
        }

        public void OnBulletHit(BaseBullet Collider)
        {
            LockedList_.Remove(Collider);
            OnHitDamage(null, Collider.Name, Collider.Damage);
        }

        public void OnHitDamage(BaseNpc Attacker, string SourceName, float Damage)
        {
            if (IsFsmState(FsmStateName.Die))
            {
                return;
            }

            if (Attacker != null)
            {
                AttackerNpc = Attacker;
            }

            var RealDamage = AddAttr(NpcAttrIndex.Hp, -Damage);
            EventManager.Send(new NpcDamageEvent(ID, Team, Attacker?.ID ?? 0, SourceName, Mathf.Abs(RealDamage), Mathf.Abs(Damage)));
            LabelManager.AddNumberLabel(Position, NumberLabelType.Float, Damage);
        }

        public void Dead()
        {
            if (IsFsmState(FsmStateName.Die))
            {
                return;
            }

            Attr.SetValue(NpcAttrIndex.Hp, 0, false);
            EventManager.Send(new NpcDieEvent(ID, Team));
        }

        public void SetDead()
        {
            IsAlive = false;
        }

        public void TurnToTarget()
        {
            if (IsValidTarget())
            {
                SetDirection(CombatHelper.CalcDirection(Position, TargetNpc.Position));
            }
        }
    }
}