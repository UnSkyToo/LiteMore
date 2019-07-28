namespace LiteMore.Combat
{
    public enum NpcAttrIndex : byte
    {
        Speed = 0,  // 移动速度
        Hp,         // 当前生命值
        MaxHp,      // 最大生命值
        AddHp,      // 生命恢复
        Mp,         // 当前魔法值
        MaxMp,      // 最大魔法值
        AddMp,      // 魔法恢复
        Damage,     // 伤害
        Gem,        // 死亡奖励宝石
        Range,      // 攻击范围
        Radius,     // 可攻击半径
        Count
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
        Attack,
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