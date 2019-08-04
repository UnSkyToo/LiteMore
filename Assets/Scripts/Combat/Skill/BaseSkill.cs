using System.Collections.Generic;
using LiteMore.Combat.Skill.Executor;
using LiteMore.Core;
using LiteMore.Player;

namespace LiteMore.Combat.Skill
{
    public class BaseSkill : BaseEntity
    {
        public uint SkillID { get; }
        public float CD { get; }
        public int Cost { get; }
        public float Radius { get; }
        public uint Level { get; }
        public float Time { get; protected set; }
        public bool IsCD { get; protected set; }

        protected readonly BaseExecutor Executor_;

        public BaseSkill(SkillDescriptor Desc)
            : base(Desc.Name)
        {
            this.SkillID = Desc.SkillID;
            this.CD = Desc.CD;
            this.Cost = Desc.Cost;
            this.Radius = Desc.Radius;
            this.Level = 1;
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
            IsCD = false;
        }

        public virtual bool CanUse()
        {
            return !IsCD && Cost <= PlayerManager.Player.Mp;
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

            if (Executor_.Execute(Name, CreateExecutorArgs(Args)))
            {
                StartCD();
                PlayerManager.AddMp(-Cost);
                return true;
            }

            return false;
        }

        protected virtual Dictionary<string, object> CreateExecutorArgs(Dictionary<string, object> Args)
        {
            var BaseArgs = new Dictionary<string, object>();
            BaseArgs.Add("SkillID", SkillID);
            BaseArgs.Add("CD", CD);
            BaseArgs.Add("Cost", Cost);
            BaseArgs.Add("Level", Level);

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