using LiteMore.Combat.Npc;

namespace LiteMore.Combat.Skill
{
    public class NpcSkill : BaseSkill
    {
        protected readonly BaseNpc Master_;

        public NpcSkill(SkillDescriptor Desc, BaseNpc Master)
            : base(Desc)
        {
            this.Master_ = Master;
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);
        }
    }
}