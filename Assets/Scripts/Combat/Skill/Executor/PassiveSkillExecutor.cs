using System.Collections.Generic;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    public abstract class PassiveExecutor : BaseExecutor
    {
        public abstract void Cancel(string Name, Dictionary<string, object> Args);
    }

    // 荆棘
    public class SkillExecutor_0001 : PassiveExecutor
    {
        private string Name_;
        private uint Level_;

        public SkillExecutor_0001()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            Name_ = Name;
            Level_ = (uint) (Args["Level"]);
            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
            return true;
        }

        public override void Cancel(string Name, Dictionary<string, object> Args)
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
        }

        private void OnNpcDamageEvent(NpcDamageEvent Event)
        {
            if (Event.Attacker == null || Event.Attacker.Team == CombatTeam.A)
            {
                return;
            }

            Event.Attacker.OnHitDamage(Event.Master, Name_, Event.Damage * Level_ * 0.5f);
        }
    }
}
