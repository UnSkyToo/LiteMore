using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore
{
    public class Skill
    {
        public event System.Action OnClick;

        private readonly Transform Transform_;
        private Image Mask_;
        private GameObject BG_;
        private Text NameText_;
        private Text CDText_;
        private Text CostText_;

        private float Time_;
        private readonly float MaxTime_;
        private bool IsCD_;

        private readonly int Cost_;

        public Skill(Transform Trans, string Name, int CD, int Cost)
        {
            Transform_ = Trans;
            MaxTime_ = CD;
            Time_ = MaxTime_;
            IsCD_ = false;

            Cost_ = Cost;

            BG_ = Transform_.Find("BG").gameObject;
            BG_.SetActive(false);
            Mask_ = Transform_.Find("Mask").GetComponent<Image>();
            NameText_ = Transform_.Find("Name").GetComponent<Text>();
            NameText_.text = Name;
            CDText_ = Transform_.Find("CD").GetComponent<Text>();
            CostText_ = Transform_.Find("Cost").GetComponent<Text>();
            CostText_.text = $"Cost:{Cost}";

            ClearCD();

            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.Click, (Obj) =>
            {
                Use();
            });
        }

        public void Destroy()
        {
            Object.Destroy(Transform_.gameObject);
            UIEventTriggerListener.Remove(Transform_);
        }

        public void Tick(float DeltaTime)
        {
            if (PlayerManager.Mp < Cost_)
            {
                BG_.SetActive(true);
            }
            else
            {
                BG_.SetActive(false);
            }

            if (!IsCD_)
            {
                return;
            }

            Time_ -= DeltaTime;
            if (Time_ <= 0.0f)
            {
                Time_ = 0;
                ClearCD();
            }

            Mask_.fillAmount = (1 - Time_ / MaxTime_);
            CDText_.text = $"{Time_:0.0}s";
        }

        public void StartCD()
        {
            IsCD_ = true;
            Time_ = MaxTime_;
            CDText_.gameObject.SetActive(true);
            CDText_.text = $"{Time_:0.0}s";
            Mask_.fillAmount = 0;
        }

        public void ClearCD()
        {
            IsCD_ = false;
            CDText_.gameObject.SetActive(false);
            Mask_.fillAmount = 1;
        }

        private void Use()
        {
            if (IsCD_)
            {
                return;
            }

            if (PlayerManager.Mp < Cost_)
            {
                return;
            }

            StartCD();
            PlayerManager.AddMp(-Cost_);
            OnClick?.Invoke();
        }
    }


    public static class SkillManager
    {
        private static Transform SkillRoot_;
        private static GameObject ModelPrefab_;
        private static readonly List<Skill> SkillList_ = new List<Skill>();

        public static bool Startup()
        {
            SkillRoot_ = GameObject.Find("Skill").transform;

            ModelPrefab_ = Resources.Load<GameObject>("Prefabs/SkillIcon");
            if (ModelPrefab_ == null)
            {
                Debug.Log("SkillManager : null model prefab");
                return false;
            }

            SkillList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in SkillList_)
            {
                Entity.Destroy();
            }
            SkillList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            foreach (var Entity in SkillList_)
            {
                Entity.Tick(DeltaTime);
            }
        }

        public static Skill AddSkill(string ResName, string Name, int CD, int Cost)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(SkillRoot_, false);
            Obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);
            Obj.transform.Find("Mask").GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);

            var Entity = new Skill(Obj.transform, Name, CD, Cost);
            SkillList_.Add(Entity);

            return Entity;
        }
    }
}