using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill.Executor;

namespace LiteMore.Combat.Skill
{
    public class PassiveSkill : BaseSkill
    {
        protected bool IsSustain_;
        protected float SustainTime_;
        protected readonly PassiveExecutor PassiveExecutor_;
        protected readonly BaseNpc Master_;

        public PassiveSkill(SkillDescriptor Desc, BaseNpc Master, float SustainTime)
            : base(Desc, Master)
        {
            IsSustain_ = false;
            SustainTime_ = SustainTime;
            PassiveExecutor_ = Executor_ as PassiveExecutor;
            Master_ = Master;
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
            PassiveExecutor_?.Cancel(null);
        }

        public override bool CanUse()
        {
            return !IsSustain_;
        }

        public override bool Use(SkillArgs Args)
        {
            if (!CanUse())
            {
                return false;
            }

            if (PassiveExecutor_ == null)
            {
                return false;
            }

            if (PassiveExecutor_.Execute(Args))
            {
                IsSustain_ = true;
            }

            return IsSustain_;
        }
    }
}