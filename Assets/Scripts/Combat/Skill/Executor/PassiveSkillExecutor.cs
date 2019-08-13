using System.Collections.Generic;
using Lite.Combat.Npc.Handler;
using LiteFramework.Core.Event;
using LiteMore.Player;
using UnityEngine;

namespace LiteMore.Combat.Skill.Executor
{
    public abstract class PassiveExecutor : BaseExecutor
    {
        public abstract void Cancel(Dictionary<string, object> Args);
    }

    /// <summary>
    /// 荆棘：受到攻击反弹受到伤害的（50%，100%，150%，200%，250%）伤害
    /// </summary>
    public class SkillExecutor_1001 : PassiveExecutor
    {
        private string Name_;
        private uint SkillID_;

        public override bool Execute(Dictionary<string, object> Args)
        {
            Name_ = (string)(Args["Name"]);
            SkillID_ = (uint)(Args["SkillID"]);
            EventManager.Register<NpcDamageEvent>(OnNpcDamageEvent);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            EventManager.UnRegister<NpcDamageEvent>(OnNpcDamageEvent);
        }

        private void OnNpcDamageEvent(NpcDamageEvent Event)
        {
            if (Event.Attacker == null || Event.Attacker.Team == CombatTeam.A)
            {
                return;
            }

            var Level = SkillManager.GetSkillLevel(SkillID_);
            Event.Attacker.OnHitDamage(Event.Master, Name_, Event.RealValue * Level * 0.5f);
        }
    }

    /// <summary>
    /// 固守：减少受到伤害的（50%，60%，70%，80%，90%）
    /// </summary>
    public class SkillExecutor_1002 : PassiveExecutor
    {
        private SkillExecutor1002_NpcHandler Handler_;

        public override bool Execute(Dictionary<string, object> Args)
        {
            Handler_ = new SkillExecutor1002_NpcHandler(100, (uint)(Args["SkillID"]));
            PlayerManager.Master.RegisterHandler(Handler_);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            PlayerManager.Master.UnRegisterHandler(Handler_);
        }

        private class SkillExecutor1002_NpcHandler : BaseNpcHandler
        {
            private readonly uint SkillID_;

            public SkillExecutor1002_NpcHandler(uint Priority, uint SkillID)
                : base(Priority)
            {
                SkillID_ = SkillID;
            }

            public override float OnAddAttr(NpcAttrIndex Index, float Value)
            {
                if (Index != NpcAttrIndex.Hp || Value >= 0)
                {
                    return Value;
                }

                var Level = SkillManager.GetSkillLevel(SkillID_);
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

        public override bool Execute(Dictionary<string, object> Args)
        {
            Handler_ = new SkillExecutor1003_NpcHandler(0, (uint)(Args["SkillID"]));
            PlayerManager.Master.RegisterHandler(Handler_);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            PlayerManager.Master.UnRegisterHandler(Handler_);
        }

        private class SkillExecutor1003_NpcHandler : BaseNpcHandler
        {
            private readonly uint SkillID_;

            public SkillExecutor1003_NpcHandler(uint Priority, uint SkillID)
                : base(Priority)
            {
                SkillID_ = SkillID;
            }

            public override float OnAddAttr(NpcAttrIndex Index, float Value)
            {
                if (Index != NpcAttrIndex.Hp || Value >= 0)
                {
                    return Value;
                }

                var Level = SkillManager.GetSkillLevel(SkillID_);
                return Mathf.Clamp(Value + 10 * Level, Value, 0);
            }
        }
    }

    /// <summary>
    /// 活力：提高每秒生命恢复（10，20，30，40，50）
    /// </summary>
    public class SkillExecutor_1004 : PassiveExecutor
    {
        public override bool Execute(Dictionary<string, object> Args)
        {
            var Level = SkillManager.GetSkillLevel((uint)Args["SkillID"]);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddHp, Level * 10);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            var Level = SkillManager.GetSkillLevel((uint)Args["SkillID"]);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddHp, -Level * 10);
        }
    }

    /// <summary>
    /// 冥想：提高每秒能量恢复（10，20，30，40，50）
    /// </summary>
    public class SkillExecutor_1005 : PassiveExecutor
    {
        public override bool Execute(Dictionary<string, object> Args)
        {
            var Level = SkillManager.GetSkillLevel((uint)Args["SkillID"]);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddMp, Level * 10);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            var Level = SkillManager.GetSkillLevel((uint)Args["SkillID"]);
            PlayerManager.Master.Attr.ApplyBase(NpcAttrIndex.AddMp, -Level * 10);
        }
    }

    /// <summary>
    /// 清醒：每次释放技能有（10%，20%，30%，40%，50%）概率返还能量(5，10，15，20，25)
    /// </summary>
    public class SkillExecutor_1006 : PassiveExecutor
    {
        private uint SkillID_;

        public override bool Execute(Dictionary<string, object> Args)
        {
            SkillID_ = (uint)(Args["SkillID"]);
            EventManager.Register<UseSkillEvent>(OnUseSkillEvent);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            EventManager.UnRegister<UseSkillEvent>(OnUseSkillEvent);
        }

        private void OnUseSkillEvent(UseSkillEvent Event)
        {
            var Level = SkillManager.GetSkillLevel(SkillID_);
            if (Random.Range(0, 100) < Level * 10)
            {
                Event.Master.AddAttr(NpcAttrIndex.Mp, Level * 5);
            }
        }
    }

    /// <summary>
    /// 魔术：每次释放技能有（5%，10%，15%，20%，25%）概率不进入冷却
    /// </summary>
    public class SkillExecutor_1007 : PassiveExecutor
    {
        private uint SkillID_;

        public override bool Execute(Dictionary<string, object> Args)
        {
            SkillID_ = (uint)(Args["SkillID"]);
            EventManager.Register<UseSkillEvent>(OnUseSkillEvent);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            EventManager.UnRegister<UseSkillEvent>(OnUseSkillEvent);
        }

        private void OnUseSkillEvent(UseSkillEvent Event)
        {
            var Level = SkillManager.GetSkillLevel(SkillID_);
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
        private uint Level_;

        public override bool Execute(Dictionary<string, object> Args)
        {
            Level_ = SkillManager.GetSkillLevel((uint)(Args["SkillID"]));
            CombatManager.Calculator.AddGlobalPercent(0.05f * Level_);
            return true;
        }

        public override void Cancel(Dictionary<string, object> Args)
        {
            CombatManager.Calculator.AddGlobalPercent(0.05f * Level_);
        }
    }
}