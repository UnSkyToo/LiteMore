using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill.Executor;
using LiteMore.Core;

namespace LiteMore.Combat.Skill
{
    public abstract class BaseSkill : BaseEntity
    {
        public BaseNpc Master { get; }
        public uint SkillID { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; set; }
        public float Time { get; protected set; }
        public bool IsCD { get; protected set; }

        protected readonly BaseExecutor Executor_;

        protected BaseSkill(SkillDescriptor Desc, BaseNpc Master)
            : base(Desc.Name)
        {
            this.Master = Master;
            this.SkillID = Desc.SkillID;
            this.CD = Desc.CD;
            this.Cost = Desc.Cost;
            this.Radius = Desc.Radius;
            this.Time = 0;
            this.IsCD = false;
            this.Executor_ = Desc.Executor;
        }

        public override void Dispose()
        {
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsCD || !IsAlive)
            {
                return;
            }

            Time -= DeltaTime;
            if (Time <= 0.0f)
            {
                Time = 0;
                ClearCD();
            }
        }

        public virtual void StartCD()
        {
            Time = CD;
            IsCD = Time > 0;
        }

        public virtual void ClearCD()
        {
            Time = 0;
            IsCD = false;
        }

        public virtual bool CanUse()
        {
            if (IsCD)
            {
                return false;
            }

            if (Master != null)
            {
                return Cost <= Master.CalcFinalAttr(NpcAttrIndex.Mp);
            }

            return true;
        }

        public virtual bool Use(Dictionary<string, object> Args)
        {
            if (!CanUse())
            {
                return false;
            }

            if (Executor_ == null)
            {
                return false;
            }

            if (Executor_.Execute(CreateExecutorArgs(Args)))
            {
                StartCD();
                Master.AddAttr(NpcAttrIndex.Mp, -Cost);
                return true;
            }

            return false;
        }

        protected virtual Dictionary<string, object> CreateExecutorArgs(Dictionary<string, object> Args)
        {
            var BaseArgs = new Dictionary<string, object>();
            BaseArgs.Add("Name", Name);
            BaseArgs.Add("SkillID", SkillID);
            BaseArgs.Add("CD", CD);
            BaseArgs.Add("Cost", Cost);
            BaseArgs.Add("Master", Master);
            BaseArgs.Add("Radius", Radius);

            if (Args != null)
            {
                foreach (var Entity in Args)
                {
                    if (BaseArgs.ContainsKey(Entity.Key))
                    {
                        BaseArgs[Entity.Key] = Entity.Value;
                    }
                    else
                    {
                        BaseArgs.Add(Entity.Key, Entity.Value);
                    }
                }
            }

            return BaseArgs;
        }
    }
}