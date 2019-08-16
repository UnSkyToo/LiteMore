namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc
    {
        public NpcAttribute Attr { get; }
        private int State_;

        public float CalcFinalAttr(NpcAttrIndex Index)
        {
            return Attr.CalcFinalValue(Index);
        }

        public float AddAttr(NpcAttrIndex Index, float Value)
        {
            var RealValue = Value;
            foreach (var Handler in HandlerList_)
            {
                RealValue = Handler.OnAddAttr(Index, RealValue);
            }

            Attr.AddValue(Index, RealValue);
            return Value;
        }

        private void UpdateAttr(float DeltaTime)
        {
            var Hp = Attr.CalcFinalValue(NpcAttrIndex.Hp) + Attr.CalcFinalValue(NpcAttrIndex.AddHp) * DeltaTime;
            var MaxHp = Attr.CalcFinalValue(NpcAttrIndex.MaxHp);
            if (Hp > MaxHp)
            {
                Hp = MaxHp;
            }
            Attr.SetValue(NpcAttrIndex.Hp, Hp);

            var Mp = Attr.CalcFinalValue(NpcAttrIndex.Mp) + Attr.CalcFinalValue(NpcAttrIndex.AddMp) * DeltaTime;
            var MaxMp = Attr.CalcFinalValue(NpcAttrIndex.MaxMp);
            if (Mp > MaxMp)
            {
                Mp = MaxMp;
            }
            Attr.SetValue(NpcAttrIndex.Mp, Mp);
        }

        private void OnAttrChanged(NpcAttrIndex Index)
        {
            switch (Index)
            {
                case NpcAttrIndex.MaxHp:
                    Bar_.SetMaxHp(Attr.CalcFinalValue(NpcAttrIndex.MaxHp));
                    break;
                case NpcAttrIndex.MaxMp:
                    Bar_.SetMaxMp(Attr.CalcFinalValue(NpcAttrIndex.MaxMp));
                    break;
                case NpcAttrIndex.Speed:
                    MoveTo(TargetPos_);
                    break;
                case NpcAttrIndex.Hp:
                    if (Attr.CalcFinalValue(NpcAttrIndex.Hp) <= 0)
                    {
                        Dead();
                    }
                    break;
                default:
                    break;
            }
        }

        public bool IsState(NpcState State)
        {
            return (State_ & (int)State) == (int)State;
        }

        public bool IsNegativeState(NpcState State)
        {
            switch (State)
            {
                case NpcState.Dizzy:
                case NpcState.Silent:
                case NpcState.Taunt:
                    return true;
                case NpcState.God:
                    return false;
            }

            return false;
        }

        public void ClearState()
        {
            State_ = 0;
        }

        public void AttachState(NpcState State)
        {
            if (IsNegativeState(State) && IsState(NpcState.God))
            {
                return;
            }

            State_ |= (int)State;
        }

        public void DetachState(NpcState State)
        {
            if (State == NpcState.Dizzy)
            {
                Fsm_.ChangeToIdleState();
            }

            State_ &= (~(int)State);
        }
    }
}