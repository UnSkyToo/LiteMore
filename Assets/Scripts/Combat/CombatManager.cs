using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;

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

            return true;
        }

        public static void Shutdown()
        {
            SkillManager.Shutdown();
            EmitterManager.Shutdown();
            BulletManager.Shutdown();
            NpcManager.Shutdown();
            MapManager.Shutdown();
            SfxManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            SfxManager.Tick(DeltaTime);
            MapManager.Tick(DeltaTime);
            NpcManager.Tick(DeltaTime);
            BulletManager.Tick(DeltaTime);
            EmitterManager.Tick(DeltaTime);
            SkillManager.Tick(DeltaTime);
        }
    }
}