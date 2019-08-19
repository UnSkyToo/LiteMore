using System.Collections.Generic;
using LiteMore.Combat.Npc.Handler;
using LiteFramework.Core.Event;
using LiteMore.Combat.Npc;
using LiteMore.Player;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    public abstract class PassiveExecutor : BaseExecutor
    {
        public abstract void Cancel(SkillArgs Args);
    }

    /// <summary>
    /// 荆棘：受到攻击反弹受到伤害的（50%，100%，150%，200%，250%）伤害
    /// </summary>
    public class SkillExecutor_1001 : PassiveExecutor
    {
        private SkillArgs Args_;

        public override bool Execute(SkillArgs Args)
        {
            Args_ = Args;
            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
        }

        private void OnNpcDamageEvent(NpcDamageEvent Event)
        {
            if (Event.MasterID != Args_.Skill.Master.ID)
            {
                return;
            }

            var Attacker = NpcManager.FindNpc(Event.AttackerID);
            if (Attacker == null)
            {
                return;
            }

            var Level = Args_.Skill.Master.GetSkillLevel(Args_.Skill.SkillID);
            Attacker.OnHitDamage(Args_.Skill.Master, Args_.Skill.Name, Event.RealValue * Level * 0.5f);
        }
    }

    /// <summary>
    /// 固守：减少受到伤害的（50%，60%，70%，80%，90%）
    /// </summary>
    public class SkillExecutor_1002 : PassiveExecutor
    {
        private SkillExecutor1002_NpcHandler Handler_;

        public override bool Execute(SkillArgs Args)
        {
            Handler_ = new SkillExecutor1002_NpcHandler(100, Args.Skill.SkillID, Args.Skill.Master);
            PlayerManager.Master.RegisterHandler(Handler_);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            PlayerManager.Master.UnRegisterHandler(Handler_);
        }

        private class SkillExecutor1002_NpcHandler : BaseNpcHandler
        {
            private readonly uint SkillID_;
            private readonly BaseNpc Master_;

            public SkillExecutor1002_NpcHandler(uint Priority, uint SkillID, BaseNpc Master)
                : base(Priority)
            {
                SkillID_ = SkillID;
                Master_ = Master;
            }

            public override float OnAddAttr(NpcAttrIndex Index, float Value)
            {
                if (Index != NpcAttrIndex.Hp || Value >= 0)
                {
                    return Value;
                }

                var Level = Master_.GetSkillLevel(SkillID_);
                return Value * (1 - (0.4f + 0.1f * Level));
            }
        }
    }

    /// <summary>
    /// 铠甲：固定减少（10,20,30,40,50）点受到的伤害
    /// </summary>
    public class SkillExecutor_1003 : PassiveExecutor
    {
        private SkillExecutor1003_NpcHandler Handler_;

        public override bool Execute(SkillArgs Args)
        {
            Handler_ = new SkillExecutor1003_NpcHandler(0, Args.Skill.SkillID, Args.Skill.Master);
            PlayerManager.Master.RegisterHandler(Handler_);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            PlayerManager.Master.UnRegisterHandler(Handler_);
        }

        private class SkillExecutor1003_NpcHandler : BaseNpcHandler
        {
            private readonly uint SkillID_;
            private readonly BaseNpc Master_;

            public SkillExecutor1003_NpcHandler(uint Priority, uint SkillID, BaseNpc Master)
                : base(Priority)
            {
                SkillID_ = SkillID;
                Master_ = Master;
            }

            public override float OnAddAttr(NpcAttrIndex Index, float Value)
            {
                if (Index != NpcAttrIndex.Hp || Value >= 0)
                {
                    return Value;
                }

                var Level = Master_.GetSkillLevel(SkillID_);
                return Mathf.Clamp(Value + 10 * Level, Value, 0);
            }
        }
    }

    /// <summary>
    /// 活力：提高每秒生命恢复（10，20，30，40，50）
    /// </summary>
    public class SkillExecutor_1004 : PassiveExecutor
    {
        private BaseNpc Master_;
        private uint SkillID_;

        public override bool Execute(SkillArgs Args)
        {
            Master_ = Args.Skill.Master;
            SkillID_ = Args.Skill.SkillID;
            var Level = Master_.GetSkillLevel(SkillID_);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddHp, Level * 10);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            var Level = Master_.GetSkillLevel(SkillID_);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddHp, -Level * 10);
        }
    }

    /// <summary>
    /// 冥想：提高每秒能量恢复（10，20，30，40，50）
    /// </summary>
    public class SkillExecutor_1005 : PassiveExecutor
    {
        private BaseNpc Master_;
        private uint SkillID_;

        public override bool Execute(SkillArgs Args)
        {
            Master_ = Args.Skill.Master;
            SkillID_ = Args.Skill.SkillID;
            var Level = Master_.GetSkillLevel(SkillID_);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddMp, Level * 10);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            var Level = Master_.GetSkillLevel(SkillID_);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddMp, -Level * 10);
        }
    }

    /// <summary>
    /// 清醒：每次释放技能有（10%，20%，30%，40%，50%）概率返还能量(5，10，15，20，25)
    /// </summary>
    public class SkillExecutor_1006 : PassiveExecutor
    {
        private BaseNpc Master_;
        private uint SkillID_;

        public override bool Execute(SkillArgs Args)
        {
            Master_ = Args.Skill.Master;
            SkillID_ = Args.Skill.SkillID;
            EventManager.Register<NpcSkillEvent>(OnNpcSkillEvent);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            EventManager.UnRegister<NpcSkillEvent>(OnNpcSkillEvent);
        }

        private void OnNpcSkillEvent(NpcSkillEvent Event)
        {
            if (Event.MasterID != Master_.ID)
            {
                return;
            }

            var Level = Master_.GetSkillLevel(SkillID_);
            if (Random.Range(0, 100) < Level * 10)
            {
                Master_.AddAttr(NpcAttrIndex.Mp, Level * 5);
            }
        }
    }

    /// <summary>
    /// 魔术：每次释放技能有（5%，10%，15%，20%，25%）概率不进入冷却
    /// </summary>
    public class SkillExecutor_1007 : PassiveExecutor
    {
        private BaseNpc Master_;
        private uint SkillID_;

        public override bool Execute(SkillArgs Args)
        {
            Master_ = Args.Skill.Master;
            SkillID_ = Args.Skill.SkillID;
            EventManager.Register<NpcSkillEvent>(OnNpcSkillEvent);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            EventManager.UnRegister<NpcSkillEvent>(OnNpcSkillEvent);
        }

        private void OnNpcSkillEvent(NpcSkillEvent Event)
        {
            var Level = Master_.GetSkillLevel(SkillID_);
            if (Random.Range(0, 100) < Level * 5)
            {
                SkillManager.FindSkill(Event.SkillID)?.ClearCD();
            }
        }
    }

    /// <summary>
    /// 大师：提高所有主动技能（5%，10%，15%，20%，30%）效果
    /// </summary>
    public class SkillExecutor_1008 : PassiveExecutor
    {
        private BaseNpc Master_;
        private uint Level_;

        public override bool Execute(SkillArgs Args)
        {
            Master_ = Args.Skill.Master;
            Level_ = Master_.GetSkillLevel(Args.Skill.SkillID);
            CombatManager.Calculator.AddGlobalPercent(0.05f * Level_);
            return true;
        }

        public override void Cancel(SkillArgs Args)
        {
            CombatManager.Calculator.AddGlobalPercent(0.05f * Level_);
        }
    }
}