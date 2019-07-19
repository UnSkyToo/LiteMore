using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore
{
    public abstract class NpcEvent : EventBase
    {
        public uint ID { get; }

        protected NpcEvent(uint ID)
        {
            this.ID = ID;
        }
    }

    public class IdleEvent : NpcEvent
    {
        public IdleEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class MoveEvent : NpcEvent
    {
        public Vector2 TargetPos { get; }

        public MoveEvent(uint ID, Vector2 TargetPos)
            : base(ID)
        {
            this.TargetPos = TargetPos;
        }
    }

    public class AttackEvent : NpcEvent
    {
        public AttackEvent(uint ID)
            : base(ID)
        {
        }
    }

    public class DieEvent : NpcEvent
    {
        public DieEvent(uint ID)
            : base(ID)
        {
        }
    }

    public enum NpcFsmStateName
    {
        Idle,
        Walk,
        Attack,
        Die
    }

    public abstract class NpcFsmStateBase
    {
        public NpcFsmStateName StateName { get; }
        public NpcFsm Fsm { get; }

        protected NpcFsmStateBase(NpcFsmStateName StateName, NpcFsm Fsm)
        {
            this.StateName = StateName;
            this.Fsm = Fsm;
        }

        public virtual void OnEnter(NpcEvent Event) { }
        public virtual void OnExit() { }
        public virtual void OnTick(float DeltaTime) { }
        public virtual void OnEvent(NpcEvent Event) { }
    }

    public class NpcFsmStateIdle : NpcFsmStateBase
    {
        public NpcFsmStateIdle(NpcFsm Fsm)
            : base(NpcFsmStateName.Idle, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is MoveEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Walk, Event);
            }
            else if (Event is DieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }

    public class NpcFsmStateWalk : NpcFsmStateBase
    {
        private Vector2 BeginPos_;
        private Vector2 EndPos_;
        private float MoveTime_;
        private float MoveTotalTime_;
        private bool IsMove_;

        public NpcFsmStateWalk(NpcFsm Fsm)
            : base(NpcFsmStateName.Walk, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            BeginPos_ = Fsm.Master.Position;
            EndPos_ = (Event as MoveEvent).TargetPos;
            MoveTime_ = 0;
            MoveTotalTime_ = (EndPos_ - BeginPos_).magnitude / Fsm.Master.Speed;
            IsMove_ = true;

            Fsm.Master.PlayAnimation("Walk", true);
        }

        public override void OnTick(float DeltaTime)
        {
            if (!IsMove_)
            {
                Fsm.ChangeToState(NpcFsmStateName.Attack, null);
                return;
            }

            MoveTime_ += DeltaTime;
            var T = MoveTime_ / MoveTotalTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                IsMove_ = false;
            }

            Fsm.Master.Position = Vector2.Lerp(BeginPos_, EndPos_, T);
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is MoveEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Walk, Event);
            }
            else if (Event is IdleEvent)
            {
                Fsm.ChangeToIdleState();
            }
            else if (Event is DieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }

    public class NpcFsmStateAttack : NpcFsmStateBase
    {
        public NpcFsmStateAttack(NpcFsm Fsm)
            : base(NpcFsmStateName.Attack, Fsm)
        {
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Idle", true);

            PlayerManager.AddHp(-Fsm.Master.Damage);
            Fsm.ChangeToState(NpcFsmStateName.Die, new DieEvent(Fsm.Master.ID));
        }

        public override void OnEvent(NpcEvent Event)
        {
            if (Event is DieEvent)
            {
                Fsm.ChangeToState(NpcFsmStateName.Die, Event);
            }
        }
    }

    public class NpcFsmStateDie : NpcFsmStateBase
    {
        private bool FadeOut_;
        private float FadeOutTime_;
        private float FadeOutMaxTime_;
        private readonly SpriteRenderer SpriteRenderer_;
        private Color SpriteColor_;
        private readonly CanvasGroup CanvasGroup_;

        public NpcFsmStateDie(NpcFsm Fsm)
            : base(NpcFsmStateName.Die, Fsm)
        {
            FadeOut_ = false;
            SpriteRenderer_ = Fsm.Master.GetComponent<SpriteRenderer>();
            SpriteColor_ = SpriteRenderer_.color;
            CanvasGroup_ = Fsm.Master.GetComponent<CanvasGroup>();
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Die", false);
        }

        public override void OnTick(float DeltaTime)
        {
            if (FadeOut_)
            {
                FadeOutTime_ += DeltaTime;
                var T = FadeOutTime_ / FadeOutMaxTime_;
                if (T >= 1.0f)
                {
                    FadeOut_ = false;
                    T = 1.0f;
                    Fsm.Master.SetDead();
                }

                SpriteColor_.a = 1 - T;
                SpriteRenderer_.color = SpriteColor_;
                CanvasGroup_.alpha = 1 - T;
                return;
            }

            if (Fsm.Master.AnimationIsEnd())
            {
                Fsm.Master.GetComponent<Animator>().speed = 0;
                FadeOut_ = true;
                FadeOutTime_ = 0;
                FadeOutMaxTime_ = 0.5f;
                return;
            }
        }
    }

    public class NpcFsm
    {
        public Npc Master { get; }

        private readonly Dictionary<NpcFsmStateName, NpcFsmStateBase> StateList_;
        private NpcFsmStateBase CurrentState_;

        public NpcFsm(Npc Master)
        {
            this.Master = Master;
            this.StateList_ = new Dictionary<NpcFsmStateName, NpcFsmStateBase>
            {
                {NpcFsmStateName.Idle, new NpcFsmStateIdle(this)},
                {NpcFsmStateName.Walk, new NpcFsmStateWalk(this)},
                {NpcFsmStateName.Attack, new NpcFsmStateAttack(this)},
                {NpcFsmStateName.Die, new NpcFsmStateDie(this)},
            };

        }

        public void Tick(float DeltaTime)
        {
            CurrentState_?.OnTick(DeltaTime);
        }

        public void OnEvent(NpcEvent Event)
        {
            CurrentState_?.OnEvent(Event);
        }

        public void ChangeToState(NpcFsmStateName StateName, NpcEvent Event)
        {
            CurrentState_?.OnExit();
            CurrentState_ = StateList_[StateName];
            CurrentState_?.OnEnter(Event);
        }

        public void ChangeToIdleState()
        {
            ChangeToState(NpcFsmStateName.Idle, null);
        }
    }

    public class Npc : GameEntity
    {
        public int Hp { get; protected set; }
        public int ForecastHp { get; protected set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public int Gem { get; set; }

        private readonly NpcFsm Fsm_;
        private string CurrentAnimName_;
        private bool CurrentAnimLoop_;

        private readonly Slider HpBar_;

        private int HitSfxInterval_;

        public Npc(Transform Trans)
            : base(Trans)
        {
            Speed = 100;
            Damage = 1;
            Gem = 1;

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
                SfxManager.AddSfx("Prefabs/GoldSfx", Position);
                Fsm_.ChangeToState(NpcFsmStateName.Die, new DieEvent(ID));
            }

            HpBar_.value = Hp;

            if (HitSfxInterval_ <= 0)
            {
                SfxManager.AddSfx("Prefabs/HitSfx", Position);
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

        public void SetHp(int Hp)
        {
            if (!IsAlive)
            {
                return;
            }

            this.Hp = Hp;
            this.ForecastHp = Hp;
            HpBar_.maxValue = Hp;
            HpBar_.minValue = 0;
            HpBar_.value = Hp;
        }
    }

    public static class NpcManager
    {
        private static Transform NpcRoot_;
        private static GameObject ModelPrefab_;
        private static List<Npc> NpcList_;

        public static bool Startup()
        {
            NpcRoot_ = GameObject.Find("Npc").transform;

            ModelPrefab_ = Resources.Load<GameObject>("Prefabs/R2");
            if (ModelPrefab_ == null)
            {
                Debug.Log("NpcManager : null model prefab");
                return false;
            }

            NpcList_ = new List<Npc>();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in NpcList_)
            {
                Entity.Destroy();
            }
            NpcList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = NpcList_.Count - 1; Index >= 0; --Index)
            {
                NpcList_[Index].Tick(DeltaTime);

                if (!NpcList_[Index].IsAlive)
                {
                    NpcList_[Index].Destroy();
                    NpcList_.RemoveAt(Index);
                }
            }
        }

        public static Npc AddNpc(Vector2 Position)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(NpcRoot_, false);
            Obj.transform.localPosition = Position;
            
            var Entity = new Npc(Obj.transform);
            Entity.Create();
            NpcList_.Add(Entity);
            Entity.Position = Position;

            return Entity;
        }

        public static int GetCount()
        {
            return NpcList_.Count;
        }

        public static List<Npc> GetNpcList()
        {
            return NpcList_;
        }

        public static Npc FindNpc(uint ID)
        {
            foreach (var Entity in NpcList_)
            {
                if (Entity.ID == ID)
                {
                    return Entity;
                }
            }

            return null;
        }

        public static Npc GetRandomNpc()
        {
            if (GetCount() == 0)
            {
                return null;
            }

            for (var Index = 0; Index < NpcList_.Count; ++Index)
            {
                if (NpcList_[Index].ForecastHp > 0)
                {
                    return NpcList_[Index];
                }
            }

            return null;
        }

        public static void OnNpcEvent(NpcEvent Event)
        {
            var Entity = FindNpc(Event.ID);
            Entity?.OnEvent(Event);
        }
    }
}