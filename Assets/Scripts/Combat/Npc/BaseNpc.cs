using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc.Handler;
using LiteMore.Combat.Npc.Module;
using LiteMore.Core;
using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class BaseNpc : GameEntity
    {
        public CombatTeam Team { get; protected set; }
        public bool IsStatic { get; set; }

        protected readonly List<BaseNpcModule> ModuleList_;
        protected readonly List<BaseNpcHandler> HandlerList_;
        protected readonly NpcBar Bar_;
        private int HitSfxInterval_;

        public NpcDataModule Data { get; }
        public NpcActorModule Actor { get; }
        public NpcActionModule Action { get; }
        public NpcSkillModule Skill { get; }

        public Transform FrontLayer { get; }
        public Transform BackLayer { get; }

        public BaseNpc(string Name, Transform Trans, CombatTeam Team, float[] InitAttr)
            : base(Name, Trans)
        {
            this.Team = Team;
            this.IsStatic = false;

            this.ModuleList_ = new List<BaseNpcModule>();
            this.HandlerList_ = new List<BaseNpcHandler>();

            FrontLayer = MiscHelper.CreateCanvasLayer(Trans, "Front", Configure.NpcFrontOrder);
            BackLayer = MiscHelper.CreateCanvasLayer(Trans, "Back", Configure.NpcBackOrder);

            EventManager.Register<NpcAttrChangedEvent>(OnNpcAttrChangedEvent);

            this.Data = AddModule(new NpcDataModule(this, InitAttr, 0));
            this.Actor = AddModule(new NpcActorModule(this));
            this.Action = AddModule(new NpcActionModule(this));
            this.Skill = AddModule(new NpcSkillModule(this));

            this.Bar_ = new NpcBar(this, Trans.Find("Bar").localPosition);
            this.Bar_.SetMaxHp(CalcFinalAttr(NpcAttrIndex.MaxHp));
            this.Bar_.SetMaxMp(CalcFinalAttr(NpcAttrIndex.MaxMp));
            this.HitSfxInterval_ = 0;

            Actor.RegisterMsg("Attack", 0.25f, CombatMsgCode.Atk);
            Actor.ChangeToIdleState();
        }

        public override string ToString()
        {
            return $"{Name}-{Team}-{ID}";
        }

        public override void Tick(float DeltaTime)
        {
            foreach (var Handler in HandlerList_)
            {
                Handler.OnTick(DeltaTime);
            }

            foreach (var Module in ModuleList_)
            {
                Module.Tick(DeltaTime);
            }

            HitSfxInterval_--;
            Bar_.Tick(DeltaTime);
            base.Tick(DeltaTime);
        }

        public override void Dispose()
        {
            Object.Destroy(BackLayer.gameObject);
            Object.Destroy(FrontLayer.gameObject);

            EventManager.UnRegister<NpcAttrChangedEvent>(OnNpcAttrChangedEvent);

            foreach (var Module in ModuleList_)
            {
                Module.Dispose();
            }

            base.Dispose();
        }

        public T AddModule<T>(T Module) where T : BaseNpcModule
        {
            if (GetModule<T>() != null)
            {
                return GetModule<T>();
            }

            ModuleList_.Add(Module);
            return Module;
        }

        public T GetModule<T>() where T : BaseNpcModule
        {
            foreach (var Module in ModuleList_)
            {
                if (Module is T MT)
                {
                    return MT;
                }
            }

            return null;
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

        public NpcBar GetBar()
        {
            return Bar_;
        }

        private void OnNpcAttrChangedEvent(NpcAttrChangedEvent Event)
        {
            if (Event.Master.ID != ID)
            {
                return;
            }

            switch (Event.Index)
            {
                case NpcAttrIndex.MaxHp:
                    Bar_.SetMaxHp(CalcFinalAttr(NpcAttrIndex.MaxHp));
                    break;
                case NpcAttrIndex.MaxMp:
                    Bar_.SetMaxMp(CalcFinalAttr(NpcAttrIndex.MaxMp));
                    break;
                case NpcAttrIndex.Speed:
                    Action.MoveTo(Action.TargetPos);
                    break;
                case NpcAttrIndex.Hp:
                    if (Event.ChangeValue < 0)
                    {
                        LabelManager.AddNumberLabel(Position, NumberLabelType.Float, Event.ChangeValue);
                    }

                    if (CalcFinalAttr(NpcAttrIndex.Hp) <= 0)
                    {
                        Dead();
                    }
                    break;
                default:
                    break;
            }
        }

        public bool IsValid()
        {
            return IsAlive && !Actor.IsFsmState(FsmStateName.Die) && CalcFinalAttr(NpcAttrIndex.Hp) > 0;
        }

        public void OnHitDamage(BaseNpc Attacker, string SourceName, float Damage)
        {
            if (Actor.IsFsmState(FsmStateName.Die))
            {
                return;
            }

            var RealDamage = AddAttr(NpcAttrIndex.Hp, -Damage);
            EventManager.Send(new NpcDamageEvent(this, Attacker?.ID ?? 0, SourceName, Mathf.Abs(RealDamage), Mathf.Abs(Damage)));
        }

        public void TryToPlayHitSfx(string HitSfx)
        {
            if (HitSfxInterval_ <= 0)
            {
                SfxManager.PlayNpcSfx(this, true, HitSfx);
                HitSfxInterval_ = 8;
            }
        }

        public float CalcFinalAttr(NpcAttrIndex Index)
        {
            return Data.CalcFinalAttr(Index);
        }

        public float AddAttr(NpcAttrIndex Index, float Value)
        {
            var RealValue = Value;
            foreach (var Handler in HandlerList_)
            {
                RealValue = Handler.OnAddAttr(Index, RealValue);
            }

            Data.AddAttr(Index, RealValue);
            return Value;
        }

        public void Dead()
        {
            if (Actor.IsFsmState(FsmStateName.Die))
            {
                return;
            }

            EventManager.Send(new NpcDieEvent(this));
        }

        public void SetDead()
        {
            IsAlive = false;
        }

        public Quaternion GetRotation()
        {
            return Rotation;
        }
    }
}