using System.Collections.Generic;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    public class SkillExecutor_1001 : BaseExecutor
    {
        public SkillExecutor_1001()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var BulletDesc = new LaserBulletDescriptor(
                new BaseBulletDescriptor(Name, Configure.CoreTopPosition, CombatTeam.A, 100),
                180, 360, 300, 500);
            BulletManager.AddLaserBullet(BulletDesc);
            return true;
        }
    }

    public class SkillExecutor_1002 : BaseExecutor
    {
        public SkillExecutor_1002()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            EmitterManager.AddEmitter(new BulletCircleEmitter(Name)
            {
                Team = CombatTeam.A,
                TriggerCount = 10,
                EmittedCount = 100,
                Interval = 30.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = Configure.CoreTopPosition,
                RadiusAttr = new EmitterRandFloat(0, 0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandFloat(3, 5),
                ResName = "Blue",
            });
            return true;
        }
    }

    public class SkillExecutor_1003 : BaseExecutor
    {
        public SkillExecutor_1003()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            EmitterManager.AddEmitter(new NpcCircleEmitter(Name)
            {
                Team = CombatTeam.B,
                TriggerCount = 1,
                EmittedCount = 100,
                Interval = 1.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Configure.WindowLeft + 200, 0),
                RadiusAttr = new EmitterRandFloat(10, 200),
                SpeedAttr = new EmitterRandFloat(80, 180),
                HpAttr = new EmitterRandFloat(1, 5),
                DamageAttr = new EmitterRandFloat(1, 1),
                GemAttr = new EmitterRandInt(1, 1),
            });
            return true;
        }
    }

    public class SkillExecutor_1004 : BaseExecutor
    {
        public SkillExecutor_1004()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var Pos = (Vector2)Args["Position"];
            var Radius = (float)Args["Radius"];

            var BulletDesc = new BombBulletDescriptor(
                new BaseBulletDescriptor(Name, new Vector2(Pos.x, 400), CombatTeam.A, 500),
                Pos, 200, Radius);

            BulletManager.AddBombBullet(BulletDesc);
            return true;
        }
    }

    public class SkillExecutor_1005 : BaseExecutor
    {
        public SkillExecutor_1005()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var Dir = (Vector2)Args["Direction"];

            var BulletDesc = new BackBulletDescriptor(
                new BaseBulletDescriptor(Name, new Vector2(Configure.CoreTopPosition.x - 50, 0), CombatTeam.A, 1),
                Dir, Configure.WindowWidth,
                new Vector2(400, 82),
                200);

            BulletManager.AddBackBullet(BulletDesc);
            return true;
        }
    }

    public class SkillExecutor_1006 : BaseExecutor
    {
        public SkillExecutor_1006()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var Pos = (Vector2)Args["Position"];
            var Radius = (float)Args["Radius"];

            var BulletDesc = new AttrTriggerBulletDescriptor(
                new BaseTriggerBulletDescriptor(
                    new BaseBulletDescriptor(Name, Pos, CombatTeam.A, 1),
                    Radius,
                    0.5f,
                    20,
                    new Color(90.0f / 255.0f, 220.0f / 255.0f, 1.0f)),
                new List<NpcAttrModifyInfo>
                {
                    new NpcAttrModifyInfo(NpcAttrIndex.Speed, 0.5f, 0),
                });

            BulletManager.AddAttrTriggerBullet(BulletDesc);
            return true;
        }
    }

    public class SkillExecutor_1007 : BaseExecutor
    {
        public SkillExecutor_1007()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var Npc = NpcManager.AddNpc(Name, Configure.CoreBasePosition, CombatTeam.A,
                NpcManager.GenerateInitAttr(200, 500, 0, 1, 0, 50, 50),
                true);
            Npc.Scale = new Vector2(3, 3);
            return true;
        }
    }

    public class SkillExecutor_1008 : BaseExecutor
    {
        public SkillExecutor_1008()
            : base()
        {
        }

        public override bool Execute(string Name, Dictionary<string, object> Args)
        {
            var Target = NpcManager.GetRandomNpc(CombatTeam.B);
            if (Target != null)
            {
                BulletManager.AddTrackBullet(new TrackBulletDescriptor(
                    new BaseBulletDescriptor(Name, Configure.CoreTopPosition, CombatTeam.A, 1),
                    "Blue",
                    Target,
                    1500));
                return true;
            }

            return false;
        }
    }
}