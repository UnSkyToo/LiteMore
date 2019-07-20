using LiteMore.Combat;
using LiteMore.UI;
using LiteMore.UI.Logic;
using UnityEngine;
using UnityEngine.UI;

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

        private static Slider HpBar_;
        private static Slider MpBar_;

        public static bool Startup()
        {
            Hp = MaxHp = 10;
            Mp = MaxMp = 100;
            Gem = 0;
            HpAdd = 20;
            MpAdd = 10;

            var HpBarObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/HpBar"));
            HpBarObj.transform.SetParent(GameObject.Find("UI").transform, false);
            HpBarObj.transform.localScale = new Vector3(2.5f, 1.5f, 1);
            HpBarObj.transform.localPosition = MapManager.BuildPosition + Vector2.up * 50;
            HpBar_ = HpBarObj.GetComponent<Slider>();
            HpBar_.minValue = 0;
            HpBar_.maxValue = MaxHp;
            HpBar_.value = MaxHp;

            var MpBarObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/MpBar"));
            MpBarObj.transform.SetParent(GameObject.Find("UI").transform, false);
            MpBarObj.transform.localScale = new Vector3(2.5f, 1.5f, 1);
            MpBarObj.transform.localPosition = MapManager.BuildPosition + Vector2.up * 30;
            MpBar_ = MpBarObj.GetComponent<Slider>();
            MpBar_.minValue = 0;
            MpBar_.maxValue = MaxMp;
            MpBar_.value = MaxMp;

            UIManager.OpenUI<PlayerInfoUI>();

            AddGem(0);
            AddHp(0);
            AddMp(0);

            return true;
        }

        public static void Shutdown()
        {
            UIManager.CloseUI<PlayerInfoUI>();

            Object.Destroy(HpBar_.gameObject);
            Object.Destroy(MpBar_.gameObject);
        }

        public static void Tick(float DeltaTime)
        {
            AddHp(HpAdd * DeltaTime);
            AddMp(MpAdd * DeltaTime);
        }

        public static void AddGem(int Value)
        {
            Gem += Value;
            EventManager.Send<PlayerGemChangeEvent>();
        }

        public static void AddHp(float Value)
        {
            if (Hp >= MaxHp)
            {
                return;
            }

            Hp = Mathf.Clamp(Hp + Value, 0, MaxHp);
            HpBar_.value = Hp;
            EventManager.Send<PlayerHpChangeEvent>();

            if (Hp <= 0)
            {
                GameManager.GameOver();
            }
        }

        public static void AddMp(float Value)
        {
            if (Mp >= MaxMp)
            {
                return;
            }

            Mp = Mathf.Clamp(Mp + Value, 0, MaxMp);
            MpBar_.value = Mp;
            EventManager.Send<PlayerMpChangeEvent>();
        }
    }
}