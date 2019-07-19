using UnityEngine;
using UnityEngine.UI;

namespace LiteMore
{
    public static class PlayerManager
    {
        public static float Hp { get; private set; }
        public static float MaxHp { get; private set; }
        public static float Mp { get; private set; }
        public static float MaxMp { get; private set; }
        public static int Gem { get; private set; }

        private static float HpAdd_;
        private static float MpAdd_;

        private static Text GemText_;
        private static Text HpText_;
        private static Text MpText_;

        private static Slider HpBar_;
        private static Slider MpBar_;

        public static bool Startup()
        {
            Hp = MaxHp = 1000;
            Mp = MaxMp = 100;
            Gem = 0;
            HpAdd_ = 50;
            MpAdd_ = 10;

            GemText_ = GameObject.Find("Gem").GetComponent<Text>();
            HpText_ = GameObject.Find("Hp").GetComponent<Text>();
            MpText_ = GameObject.Find("Mp").GetComponent<Text>();

            var HpBarObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/HpBar"));
            HpBarObj.transform.SetParent(GameObject.Find("Info").transform, false);
            HpBarObj.transform.localScale = new Vector3(2.5f, 1.5f, 1);
            HpBarObj.transform.localPosition = MapManager.BuildPosition + Vector2.up * 50;
            HpBar_ = HpBarObj.GetComponent<Slider>();
            HpBar_.minValue = 0;
            HpBar_.maxValue = MaxHp;
            HpBar_.value = MaxHp;

            var MpBarObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/MpBar"));
            MpBarObj.transform.SetParent(GameObject.Find("Info").transform, false);
            MpBarObj.transform.localScale = new Vector3(2.5f, 1.5f, 1);
            MpBarObj.transform.localPosition = MapManager.BuildPosition + Vector2.up * 30;
            MpBar_ = MpBarObj.GetComponent<Slider>();
            MpBar_.minValue = 0;
            MpBar_.maxValue = MaxMp;
            MpBar_.value = MaxMp;

            AddGem(0);
            AddHp(0);
            AddMp(0);

            return true;
        }

        public static void Shutdown()
        {
            Object.Destroy(HpBar_.gameObject);
            Object.Destroy(MpBar_.gameObject);
        }

        public static void Tick(float DeltaTime)
        {
            AddHp(HpAdd_ * DeltaTime);
            AddMp(MpAdd_ * DeltaTime);
        }

        public static void AddGem(int Gem)
        {
            PlayerManager.Gem += Gem;
            GemText_.text = $"Gem:{PlayerManager.Gem}";
        }

        public static void AddHp(float Hp)
        {
            PlayerManager.Hp += Hp;
            if (PlayerManager.Hp > PlayerManager.MaxHp)
            {
                PlayerManager.Hp = PlayerManager.MaxHp;
            }
            HpText_.text = $"Hp:{(int)PlayerManager.Hp}/{(int)PlayerManager.MaxHp}";
            HpBar_.value = PlayerManager.Hp;

            if (PlayerManager.Hp < 0)
            {
                GameManager.GameOver();
            }
        }

        public static void AddMp(float Mp)
        {
            PlayerManager.Mp += Mp;
            if (PlayerManager.Mp > PlayerManager.MaxMp)
            {
                PlayerManager.Mp = PlayerManager.MaxMp;
            }
            MpText_.text = $"Mp:{(int)PlayerManager.Mp}/{(int)PlayerManager.MaxMp}";
            MpBar_.value = PlayerManager.Mp;
        }
    }
}