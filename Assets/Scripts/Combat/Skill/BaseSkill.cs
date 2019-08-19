using LiteMore.Combat.AI.Locking;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using LiteMore.Combat.Skill.Executor;
using LiteMore.Core;

namespace LiteMore.Combat.Skill
{
    public abstract class BaseSkill : BaseEntity
    {
        public BaseNpc Master { get; }
        public uint SkillID { get; }
        public string Icon { get; }
        public float CD { get; set; }
        public int Cost { get; set; }
        public float Radius { get; set; }
        public BaseShape Shape { get; }
        public LockingRule Rule { get; }
        public float Time { get; protected set; }
        public bool IsCD { get; protected set; }

        protected readonly BaseExecutor Executor_;

        protected BaseSkill(SkillDescriptor Desc, BaseNpc Master)
            : base(Desc.Name)
        {
            this.Master = Master;
            this.SkillID = Desc.SkillID;
            this.Icon = Desc.Icon;
            this.CD = Desc.CD;
            this.Cost = Desc.Cost;
            this.Radius = Desc.Radius;
            this.Shape = Desc.Shape;
            this.Rule = Desc.Rule;
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

        public void StartCD()
        {
            Time = CD;
            IsCD = Time > 0;
        }

        public void ClearCD()
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

        public virtual bool Use(SkillArgs Args)
        {
            if (!CanUse())
            {
                return false;
            }

            if (Executor_ == null)
            {
                return false;
            }

            if (Executor_.Execute(Args))
            {
                StartCD();
                Master.AddAttr(NpcAttrIndex.Mp, -Cost);
                return true;
            }

            return false;
        }
    }
}