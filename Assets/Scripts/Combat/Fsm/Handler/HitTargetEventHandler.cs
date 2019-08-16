using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;

namespace LiteMore.Combat.Fsm.Handler
{
    public class HitTargetEventHandler : BaseFsmHandler
    {
        public HitTargetEventHandler(BaseFsm Fsm)
            : base(Fsm)
        {
        }

        public override bool OnCombatEvent(CombatEvent Event)
        {
            var Evt = Event as NpcHitTargetEvent;
            if (Evt == null)
            {
                return false;
            }

            var Attacker = NpcManager.FindNpc(Evt.MasterTeam, Evt.MasterID);
            if (Attacker == null || !Attacker.IsValidTarget())
            {
                return false;
            }

            var Skill = Attacker.GetSkill(Evt.SkillID);
            if (!(Skill is NpcSkill))
            {
                return false;
            }

            Attacker.TargetNpc.TryToPlayHitSfx(Evt.HitSfx);
            return (Skill as NpcSkill).Use(null);
        }
    }
}