namespace LiteMore.Combat.Npc.Module
{
    public class NpcDataModule : BaseNpcModule
    {
        public NpcAttribute Attr { get; }
        private int State_;

        public NpcDataModule(BaseNpc Master, float[] InitAttr, int State)
            : base(Master)
        {
            this.Attr = new NpcAttribute(Master, InitAttr);
            this.State_ = State;
        }

        public override void Tick(float DeltaTime)
        {
            var AddHp = Attr.CalcFinalValue(NpcAttrIndex.AddHp) * DeltaTime;
            var MaxHp = Attr.CalcFinalValue(NpcAttrIndex.MaxHp);
            if (AddHp + Attr.CalcFinalValue(NpcAttrIndex.Hp) > MaxHp)
            {
                AddHp = MaxHp - Attr.CalcFinalValue(NpcAttrIndex.Hp);
            }
            Attr.AddValue(NpcAttrIndex.Hp, AddHp);

            var AddMp = Attr.CalcFinalValue(NpcAttrIndex.AddMp) * DeltaTime;
            var MaxMp = Attr.CalcFinalValue(NpcAttrIndex.MaxMp);
            if (AddMp + Attr.CalcFinalValue(NpcAttrIndex.Mp) > MaxMp)
            {
                AddMp = MaxMp - Attr.CalcFinalValue(NpcAttrIndex.Mp);
            }
            Attr.AddValue(NpcAttrIndex.Mp, AddMp);
        }

        public float CalcFinalAttr(NpcAttrIndex Index)
        {
            return Attr.CalcFinalValue(Index);
        }

        public void AddAttr(NpcAttrIndex Index, float Value, bool Notify = true)
        {
            Attr.AddValue(Index, Value, Notify);
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
                Master.Actor.ChangeToIdleState();
            }

            State_ &= (~(int)State);
        }
    }
}