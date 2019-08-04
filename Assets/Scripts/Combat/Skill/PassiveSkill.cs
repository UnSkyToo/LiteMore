using System.Collections.Generic;
using LiteMore.Combat.Skill.Executor;

namespace LiteMore.Combat.Skill
{
    public class PassiveSkill : BaseSkill
    {
        protected bool IsSustain_;
        protected float SustainTime_;
        protected readonly PassiveExecutor PassiveExecutor_;

        public PassiveSkill(SkillDescriptor Desc, float SustainTime)
            : base(Desc)
        {
            IsSustain_ = false;
            SustainTime_ = SustainTime;
            PassiveExecutor_ = Executor_ as PassiveExecutor;
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (SustainTime_ > 0)
            {
                SustainTime_ -= DeltaTime;

                if (SustainTime_ <= 0)
                {
                    IsAlive = false;
                }
            }
        }

        public override void Dispose()
        {
            IsSustain_ = false;
            PassiveExecutor_?.Cancel(Name, CreateExecutorArgs(null));
        }

        public override bool CanUse()
        {
            return !IsSustain_;
        }

        public override bool Use(Dictionary<string, object> Args)
        {
            if (!CanUse())
            {
                return false;
            }

            if (PassiveExecutor_ == null)
            {
                return false;
            }

            if (PassiveExecutor_.Execute(Name, CreateExecutorArgs(Args)))
            {
                IsSustain_ = true;
            }

            return IsSustain_;
        }
    }
}