using LiteMore.Combat;
using LiteMore.Combat.Npc;
using LiteMore.UI;
using LiteMore.UI.Logic;

namespace LiteMore
{
    public static class PlayerManager
    {
        public static float Hp { get; private set; }
        public static float MaxHp { get; private set; }
        public static float HpAdd { get; private set; }

        public static float Mp { get; private set; }
        public static float MaxMp { get; private set; }
        public static float MpAdd { get; private set; }

        public static int Gem { get; private set; }

        private static CoreNpc CoreNpc_;

        public static bool Startup()
        {
            Hp = MaxHp = 10;
            Mp = MaxMp = 100;
            Gem = 0;
            HpAdd = 20;
            MpAdd = 10;

            UIManager.OpenUI<PlayerInfoUI>();

            CoreNpc_ = NpcManager.AddCoreNpc(Configure.CoreBasePosition, NpcManager.GenerateCoreNpcAttr());
            CoreNpc_.Attr.AttrChanged += OnCoreAttrChanged;

            AddGem(0);
            AddHp(0);
            AddMp(0);

            return true;
        }

        public static void Shutdown()
        {
            UIManager.CloseUI<PlayerInfoUI>();
        }

        public static void Tick(float DeltaTime)
        {
            if (CoreNpc_.CalcFinalAttr(NpcAttrIndex.Hp) <= 0)
            {
                GameManager.GameOver();
            }
        }

        private static void OnCoreAttrChanged(NpcAttrIndex Index)
        {
            switch (Index)
            {
                case NpcAttrIndex.Hp:
                    Hp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerHpChangeEvent>();
                    break;
                case NpcAttrIndex.MaxHp:
                    MaxHp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerHpChangeEvent>();
                    break;
                case NpcAttrIndex.HpAdd:
                    HpAdd = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerHpChangeEvent>();
                    break;
                case NpcAttrIndex.Mp:
                    Mp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerMpChangeEvent>();
                    break;
                case NpcAttrIndex.MaxMp:
                    MaxMp = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerMpChangeEvent>();
                    break;
                case NpcAttrIndex.MpAdd:
                    MpAdd = CoreNpc_.CalcFinalAttr(Index);
                    EventManager.Send<PlayerMpChangeEvent>();
                    break;
                default:
                    break;
            }
        }

        public static void AddGem(int Value)
        {
            Gem += Value;
            EventManager.Send<PlayerGemChangeEvent>();
        }

        public static void AddHp(float Value)
        {
            CoreNpc_.Attr.AddValue(NpcAttrIndex.Hp, Value);
        }

        public static void AddMp(float Value)
        {
            CoreNpc_.Attr.AddValue(NpcAttrIndex.Mp, Value);
        }
    }
}