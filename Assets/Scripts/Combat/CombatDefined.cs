namespace LiteMore.Combat
{
    public enum NpcAttrIndex : byte
    {
        /// <summary>
        /// 移动速度
        /// </summary>
        Speed = 0,
        /// <summary>
        /// 当前生命值
        /// </summary>
        Hp,
        /// <summary>
        /// 最大生命值
        /// </summary>
        MaxHp,
        /// <summary>
        /// 生命恢复
        /// </summary>
        AddHp,
        /// <summary>
        /// 当前魔法值
        /// </summary>
        Mp,
        /// <summary>
        /// 最大魔法值
        /// </summary>
        MaxMp,
        /// <summary>
        /// 魔法恢复
        /// </summary>
        AddMp,
        /// <summary>
        /// 伤害
        /// </summary>
        Damage,
        /// <summary>
        /// 死亡奖励宝石
        /// </summary>
        Gem,
        /// <summary>
        /// 攻击范围
        /// </summary>
        AtkRange,
        /// <summary>
        /// 受击范围
        /// </summary>
        HitRange,
        Count
    }

    public enum NpcState : byte
    {
        /// <summary>
        /// 眩晕
        /// </summary>
        Dizzy   = 1 << 0,
        /// <summary>
        /// 沉默
        /// </summary>
        Silent  = 1 << 1,
        /// <summary>
        /// 嘲讽
        /// </summary>
        Taunt   = 1 << 2,
        /// <summary>
        /// 无敌
        /// </summary>
        God     = 1 << 3,
    }

    public enum NpcDirection : byte
    {
        None,
        Left,
        Right,
    }

    public enum FsmStateName : byte
    {
        Idle,
        Walk,
        Skill,
        Die,
        Back,
    }

    public enum CombatTeam : byte
    {
        A = 0,
        B = 1,
    }

    public enum CombatMsgCode : byte
    {
        Atk,
        Effect,
    }
}