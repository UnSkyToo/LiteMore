using LiteMore.Combat.Bullet;
using LiteMore.Combat.Npc.Fsm;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Npc
{
    public class NpcBase : GameEntity
    {
        public int Hp { get; protected set; }
        public int ForecastHp { get; protected set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public int Gem { get; set; }

        private readonly NpcFsm Fsm_;
        private readonly Animator Animator_;

        private string CurrentAnimName_;
        private bool CurrentAnimLoop_;

        private readonly Slider HpBar_;

        private int HitSfxInterval_;

        public NpcBase(Transform Trans)
            : base(Trans)
        {
            Speed = 100;
            Damage = 1;
            Gem = 1;

            Animator_ = Trans.GetComponent<Animator>();
            this.CurrentAnimName_ = string.Empty;
            this.CurrentAnimLoop_ = false;
            this.Fsm_ = new NpcFsm(this);
            this.Fsm_.ChangeToIdleState();

            HpBar_ = GetComponent<Slider>("HpBar");
            SetHp(5);
        }

        public override void Tick(float DeltaTime)
        {
            Fsm_.Tick(DeltaTime);
            base.Tick(DeltaTime);

            HitSfxInterval_--;
        }

        public void OnBulletHit(BulletBase Collider)
        {
            Hp -= Collider.Damage;

            if (Hp <= 0)
            {
                Hp = 0;
                PlayerManager.AddGem(Gem);
                SfxManager.AddSfx("Prefabs/Sfx/GoldSfx", Position);
                Fsm_.ChangeToState(NpcFsmStateName.Die, new DieEvent(ID));
            }

            HpBar_.value = Hp;

            if (HitSfxInterval_ <= 0)
            {
                SfxManager.AddSfx("Prefabs/Sfx/HitSfx", Position);
                HitSfxInterval_ = 8;
            }
        }

        public void OnLocking(BulletBase Attacker)
        {
            ForecastHp -= Attacker.Damage;
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

        public void MoveTo(Vector2 TargetPos)
        {
            Fsm_.ChangeToState(NpcFsmStateName.Walk, new MoveEvent(ID, TargetPos));
        }

        public void SetDead()
        {
            IsAlive = false;
        }

        public void SetHp(int NewHp)
        {
            if (!IsAlive)
            {
                return;
            }

            Hp = NewHp;
            ForecastHp = NewHp;
            HpBar_.maxValue = NewHp;
            HpBar_.minValue = 0;
            HpBar_.value = NewHp;
        }
    }
}