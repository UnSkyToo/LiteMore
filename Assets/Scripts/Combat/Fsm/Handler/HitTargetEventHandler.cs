using System.Collections.Generic;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using UnityEngine;

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
            /*var Evt = Event as NpcHitTargetEvent;
            if (Evt == null)
            {
                return false;
            }

            var Attacker = NpcManager.FindNpc(Evt.MasterTeam, Evt.MasterID);
            if (Attacker == null)
            {
                return false;
            }

            if (Evt.TargetList == null || Evt.TargetList.Count == 0)
            {
                return false;
            }

            var Skill = Attacker.GetSkill(Evt.SkillID);
            if (!(Skill is NpcSkill))
            {
                return false;
            }

            if ((Skill as NpcSkill).Use(new Dictionary<string, object>(){{"HitSfx", Evt.HitSfx}, {"TargetList", Evt.TargetList}}))
            {
                if (Skill.SkillID != 3001)
                {
                    LabelManager.AddStringLabel(Attacker.Position, Skill.Name, Color.green, 30);
                }

                return true;
            }*/

            return false;
        }
    }
}