using LiteMore.Combat.Bullet;
using LiteMore.Combat.Npc.Fsm;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Npc
{
    public class BaseNpc : GameEntity
    {
        public float ForecastHp { get; protected set; }

        private readonly NpcFsm Fsm_;
        private readonly Animator Animator_;

        private string CurrentAnimName_;
        private bool CurrentAnimLoop_;

        private readonly Slider HpBar_;

        private int HitSfxInterval_;
        private Vector2 TargetPos_;

        public NpcAttribute Attr { get; }

        public BaseNpc(Transform Trans, float[] InitAttr)
            : base(Trans)
        {
            Attr = new NpcAttribute(InitAttr);
            Attr.AttrChanged += OnAttrChanged;

            Animator_ = Trans.GetComponent<Animator>();
            this.CurrentAnimName_ = string.Empty;
            this.CurrentAnimLoop_ = false;
            this.Fsm_ = new NpcFsm(this);
            this.Fsm_.ChangeToIdleState();

            ForecastHp = CalcFinalAttr(NpcAttrIndex.Hp);
            HpBar_ = GetComponent<Slider>("HpBar");
            UpdateHpBar();
        }

        public override void Tick(float DeltaTime)
        {
            Fsm_.Tick(DeltaTime);
            base.Tick(DeltaTime);

            if (Fsm_.GetStateName() == NpcFsmStateName.Idle)
            {
                MoveTo(TargetPos_);
            }

            HitSfxInterval_--;
        }

        public void OnBulletHit(BaseBullet Collider)
        {
            Attr.AddValue(NpcAttrIndex.Hp, -Collider.Damage);

            if (Attr.CalcFinalValue(NpcAttrIndex.Hp) <= 0)
            {
                Attr.SetValue(NpcAttrIndex.Hp, 0);
                PlayerManager.AddGem((int)CalcFinalAttr(NpcAttrIndex.Gem));
                SfxManager.AddSfx("Prefabs/Sfx/GoldSfx", Position);
                Fsm_.OnEvent(new NpcDieEvent(ID));
            }

            HpBar_.value = Attr.CalcFinalValue(NpcAttrIndex.Hp);

            if (HitSfxInterval_ <= 0)
            {
                SfxManager.AddSfx("Prefabs/Sfx/HitSfx", Position);
                HitSfxInterval_ = 8;
            }
        }

        public void OnLocking(BaseBullet Attacker)
        {
            ForecastHp -= Attacker.Damage;
        }

        public virtual bool CanLocked()
        {
            return CalcFinalAttr(NpcAttrIndex.Hp) > 0 && ForecastHp > 0 && IsAlive;
        }

        public void PlayAnimation(string AnimName, bool IsLoop)
        {
            if (CurrentAnimName_ == AnimName && IsLoop == CurrentAnimLoop_)
            {
                return;
            }

            if (!string.IsNullOrEmpty(CurrentAnimName_))
            {
                Animator_.ResetTrigger(CurrentAnimName_);
            }

            CurrentAnimName_ = AnimName;
            CurrentAnimLoop_ = IsLoop;
            Animator_.SetTrigger(CurrentAnimName_);
        }

        public bool AnimationIsEnd()
        {
            if (CurrentAnimLoop_)
            {
                return false;
            }

            var Info = Animator_.GetCurrentAnimatorStateInfo(0);
            if (!Info.IsTag(CurrentAnimName_))
            {
                return false;
            }

            return Info.normalizedTime >= 1.0f;
        }

        public void OnEvent(NpcEvent Event)
        {
            Fsm_.OnEvent(Event);
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
                    UpdateHpBar();
                    break;
                case NpcAttrIndex.Speed:
                    MoveTo(TargetPos_);
                    break;
                default:
                    break;
            }
        }

        public void MoveTo(Vector2 TargetPos)
        {
            TargetPos_ = TargetPos;
            Fsm_.OnEvent(new NpcMoveEvent(ID, TargetPos_));
        }

        public void SetDead()
        {
            IsAlive = false;
        }

        private void UpdateHpBar()
        {
            HpBar_.maxValue = CalcFinalAttr(NpcAttrIndex.MaxHp);
            HpBar_.minValue = 0;
            HpBar_.value = CalcFinalAttr(NpcAttrIndex.Hp);
        }

        public void Back(float Distance, float Speed)
        {
            Back((Position - TargetPos_).normalized * Distance, Speed);
        }

        public void Back(Vector2 Offset, float Speed)
        {
            var BackPos = Position + Offset;
            Fsm_.OnEvent(new NpcBackEvent(ID, BackPos, Offset.magnitude / Speed));
        }
    }
}