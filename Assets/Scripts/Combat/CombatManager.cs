using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Label;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Wave;

namespace LiteMore.Combat
{
    public static class CombatManager
    {
        public static bool Startup()
        {
            if (!SfxManager.Startup())
            {
                return false;
            }

            if (!LabelManager.Startup())
            {
                return false;
            }

            if (!MapManager.Startup())
            {
                return false;
            }

            if (!NpcManager.Startup())
            {
                return false;
            }

            if (!BulletManager.Startup())
            {
                return false;
            }

            if (!EmitterManager.Startup())
            {
                return false;
            }

            if (!SkillManager.Startup())
            {
                return false;
            }

            if (!WaveManager.Startup())
            {
                return false;
            }

            EventManager.Register<NpcIdleEvent>(OnCombatEvent);
            EventManager.Register<NpcWalkEvent>(OnCombatEvent);
            EventManager.Register<NpcAttackEvent>(OnCombatEvent);
            EventManager.Register<NpcDieEvent>(OnCombatEvent);
            EventManager.Register<NpcBackEvent>(OnCombatEvent);

            return true;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<NpcIdleEvent>(OnCombatEvent);
            EventManager.UnRegister<NpcWalkEvent>(OnCombatEvent);
            EventManager.UnRegister<NpcAttackEvent>(OnCombatEvent);
            EventManager.UnRegister<NpcDieEvent>(OnCombatEvent);
            EventManager.UnRegister<NpcBackEvent>(OnCombatEvent);

            WaveManager.Shutdown();
            SkillManager.Shutdown();
            EmitterManager.Shutdown();
            BulletManager.Shutdown();
            NpcManager.Shutdown();
            MapManager.Shutdown();
            LabelManager.Shutdown();
            SfxManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            SfxManager.Tick(DeltaTime);
            LabelManager.Tick(DeltaTime);
            MapManager.Tick(DeltaTime);
            NpcManager.Tick(DeltaTime);
            BulletManager.Tick(DeltaTime);
            EmitterManager.Tick(DeltaTime);
            SkillManager.Tick(DeltaTime);
            WaveManager.Tick(DeltaTime);
        }

        private static void OnCombatEvent(CombatEvent Event)
        {
            if (Event.Master != null && Event.Master.IsAlive)
            {
                Event.Master.OnCombatEvent(Event);
            }
        }
    }
}