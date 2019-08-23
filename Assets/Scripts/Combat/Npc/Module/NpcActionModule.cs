using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Bullet;
using UnityEngine;

namespace LiteMore.Combat.Npc.Module
{
    public class NpcActionModule : BaseNpcModule
    {
        public BaseNpc TargetNpc { get; private set; }
        public BaseNpc AttackerNpc { get; set; }

        public Vector2 TargetPos { get; private set; }
        public bool IsForceMove { get; private set; }

        private readonly List<BaseBullet> LockedList_ = new List<BaseBullet>();

        public NpcActionModule(BaseNpc Master)
            : base(Master)
        {
            TargetNpc = null;
            AttackerNpc = null;

            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
        }

        public override void Dispose()
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
        }

        public override void Tick(float DeltaTime)
        {
            for (var Index = LockedList_.Count - 1; Index >= 0; --Index)
            {
                if (!LockedList_[Index].IsAlive)
                {
                    LockedList_.RemoveAt(Index);
                }
            }
        }

        private void OnNpcDamageEvent(NpcDamageEvent Event)
        {
            if (Event.Master.ID == Master.ID)
            {
                AttackerNpc = NpcManager.FindNpc(Event.AttackerID);
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
            var Hp = Master.CalcFinalAttr(NpcAttrIndex.Hp);
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
            return Master.IsAlive && !Master.Actor.IsFsmState(FsmStateName.Die) && Master.CalcFinalAttr(NpcAttrIndex.Hp) > 0 && CalcForecastHp() > 0;
        }

        public void OnBulletHit(BaseBullet Collider)
        {
            LockedList_.Remove(Collider);
            Master.OnHitDamage(null, Collider.Name, Collider.Damage);
        }

        public bool IsValidTarget()
        {
            return TargetNpc != null && TargetNpc.IsValid();
        }

        public bool IsValidAttacker()
        {
            return AttackerNpc != null && AttackerNpc.IsValid();
        }

        public void SetTarget(BaseNpc Target)
        {
            TargetNpc = Target;
        }

        public void Back(float Distance, float Speed)
        {
            Back((Master.Position - TargetPos).normalized * Distance, Speed);
        }

        public void Back(Vector2 Offset, float Speed)
        {
            if (Master.IsStatic)
            {
                return;
            }

            var BackPos = Master.Position + Offset;
            EventManager.Send(new NpcBackEvent(Master, BackPos, Offset.magnitude / Speed));
        }

        public void MoveTo(Vector2 TargetPos, bool IsForceMove = false)
        {
            if (Master.IsStatic)
            {
                return;
            }

            this.TargetPos = TargetPos;
            this.IsForceMove = IsForceMove;
            EventManager.Send(new NpcWalkEvent(Master, TargetPos));
        }

        public void StopMove()
        {
            Master.Actor.ChangeToIdleState();
            IsForceMove = false;
        }
    }
}