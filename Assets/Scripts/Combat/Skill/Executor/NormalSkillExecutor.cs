using System.Collections.Generic;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Helper;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Buff;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
using LiteMore.Player;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    /// <summary>
    /// 镭射激光
    /// </summary>
    public class SkillExecutor_2001 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var BulletDesc = new LaserBulletDescriptor(
                new BaseBulletDescriptor(Args.Skill.Name, Configure.CoreTopPosition, CombatTeam.A, 100),
                180, 360, 300, 500);
            BulletManager.AddLaserBullet(BulletDesc);
            return true;
        }
    }

    /// <summary>
    /// 自动弹幕
    /// </summary>
    public class SkillExecutor_2002 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            EmitterManager.AddEmitter(new BulletCircleEmitter(Args.Skill.Name)
            {
                Master = PlayerManager.Master,
                Team = CombatTeam.A,
                TriggerCount = 10,
                EmittedCountAttr = new EmitterRandInt(100),
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

    /// <summary>
    /// 放马过来
    /// </summary>
    public class SkillExecutor_2003 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            EmitterManager.AddEmitter(new NpcCircleEmitter(Args.Skill.Name)
            {
                Team = CombatTeam.B,
                TriggerCount = 1,
                EmittedCountAttr = new EmitterRandInt(100),
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

    /// <summary>
    /// 天降正义
    /// </summary>
    public class SkillExecutor_2004 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var BulletDesc = new BombBulletDescriptor(
                new BaseBulletDescriptor(Args.Skill.Name, new Vector2(Args.Position.x, 400), CombatTeam.A, 500),
                Args.Position, 200, Args.Skill.Radius);

            BulletManager.AddBombBullet(BulletDesc);
            return true;
        }
    }

    /// <summary>
    /// 速速退散
    /// </summary>
    public class SkillExecutor_2005 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var BulletDesc = new BackBulletDescriptor(
                new BaseBulletDescriptor(Args.Skill.Name, Args.Master.Position, CombatTeam.A, 1),
                Args.Direction, Configure.WindowWidth,
                new Vector2(400, 82),
                200);

            BulletManager.AddBackBullet(BulletDesc);
            return true;
        }
    }

    /// <summary>
    /// 减速陷阱
    /// </summary>
    public class SkillExecutor_2006 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            BuffManager.AddBuff(new TriggerBuff(new TriggerBuffDescriptor(Args.Skill.Name, 5, 0.5f, 0, true,
                new NpcAttrModifyInfo(NpcAttrIndex.Speed, 0.5f, 0), Args.Skill.Radius, 1),
                Args.Position, Args.Master.Team.Opposite()));

            return true;
        }
    }

    /// <summary>
    /// 召唤援军
    /// </summary>
    public class SkillExecutor_2007 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Offset = UnityHelper.RandVec2(100);

            var Npc = NpcManager.AddNpc(Args.Skill.Name, Configure.CoreBasePosition + Offset, CombatTeam.A, 2,
                NpcManager.GenerateInitAttr(200, 200, 0, 100, 10, 10, 0, 30, 30));
            Npc.Skill.AddNpcSkill(2006); // 减速陷阱-主动
            Npc.Skill.AddNpcSkill(2009); // 嘲讽-主动
            Npc.Skill.AddNpcSkill(2010); // 分身-主动
            Npc.Skill.AddPassiveSkill(3001, -1); // 荆棘-被动
            return true;
        }
    }

    /// <summary>
    /// 持续子弹
    /// </summary>
    public class SkillExecutor_2008 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Target = FilterHelper.FindNearest(PlayerManager.Master);
            if (Target != null)
            {
                BulletManager.AddTrackBullet(new TrackBulletDescriptor(
                    new BaseBulletDescriptor(Args.Skill.Name, Configure.CoreTopPosition, CombatTeam.A, 1),
                    "Blue",
                    Target,
                    1500));
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 嘲讽
    /// </summary>
    public class SkillExecutor_2009 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Master = Args.Master;
            if (Master == null)
            {
                return false;
            }

            var TargetList = FilterHelper.Find(Args.LockRule, new FilterArgs()
            {
                Master = Master,
                Team = Master.Team,
                Position = Master.Position,
                Radius = Args.Skill.Radius,
                Rotation = Master.Rotation,
                Shape = Args.Skill.Shape
            });

            foreach (var Target in TargetList)
            {
                if (!Target.IsValid())
                {
                    continue;
                }

                Target.Action.SetTarget(Master);
            }

            return true;
        }
    }

    /// <summary>
    /// 分身
    /// </summary>
    public class SkillExecutor_2010 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Master = Args.Master;
            if (Master == null)
            {
                return false;
            }

            var Slave = NpcManager.AddNpc($"{Master.Name}_clone", Master.Position + UnityHelper.RandVec2(100), Master.Team, Master.Scale.x, Master.Data.Attr.GetValues());
            Slave.Actor.SetDirection(Master.Actor.Direction);
            return true;
        }
    }

    /// <summary>
    /// 箭雨
    /// </summary>
    public class SkillExecutor_2011 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Level = Args.Master.Skill.GetSkillLevel(Args.Skill.SkillID);

            TimerManager.AddTimer(0.05f, () =>
            {
                var Pos = Args.Position + UnityHelper.RandVec2(Args.Skill.Radius);
                BulletManager.AddArrowBullet(
                    new ArrowBulletDescriptor(
                        new BaseBulletDescriptor(Args.Skill.Name, Args.Master.Position, Args.Master.Team, Level),
                        Pos, 1000, new Color(0.5f, 0.3f, 0.4f)));
            }, 5.0f);

            return true;
        }
    }

    /// <summary>
    /// 召唤弓箭手
    /// </summary>
    public class SkillExecutor_2012 : BaseExecutor
    {
        public override bool Execute(SkillArgs Args)
        {
            var Offset = UnityHelper.RandVec2(100);

            var Npc = NpcManager.AddNpc(Args.Skill.Name, Configure.CoreBasePosition + Offset, CombatTeam.A, 2,
                NpcManager.GenerateInitAttr(200, 200, 0, 100, 10, 1, 0, 500, 30));
            //Npc.Skill.AddNpcSkill(2011); // 箭雨-主动
            return true;
        }
    }
}