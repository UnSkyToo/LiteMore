namespace LiteMore.Combat.AI.Filter
{
    public enum FilterTeamType : byte
    {
        Self, // 自己
        Team, // 己方所有人
        TeamExceptSelf, // 己方除自己以外所有人
        Enemy, // 敌方所有人
        All, // 所有人
        Attacker, // 攻击自己的人
    }

    public enum FilterRangeType : byte
    {
        All, // 全部距离的人
        InDistance, // 指定距离内的人
        InShape, // 指定范围内的人
    }

    public enum FilterNpcType : byte
    {
        All, // 列表内所有人
        Nearest, // 列表内最近的人
        CurHpMinimum, // 列表内当前生命值最少
        CurHpMaximum, // 列表内当前生命值最多
        MaxHpMinimum, // 列表内最大生命值最少
        MaxHpMaximum, // 列表内最大生命值最多
        HpPercentMinimum, // 列表内百分比生命值最少
        HpPercentMaximum, // 列表内百分比生命值最多
        DamageMinimum, // 列表内伤害值最低
        DamageMaximum, // 列表内伤害值最高
        Random, // 列表内随机
    }
}