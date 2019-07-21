using System.Collections.Generic;
using LiteMore.Extend;
using LiteMore.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat
{
    public struct SkillDescriptor
    {
        public string Name { get; }
        public float CD { get; }
        public int Cost { get; }

        public SkillDescriptor(string Name, float CD, int Cost)
        {
            this.Name = Name;
            this.CD = CD;
            this.Cost = Cost;
        }
    }

    public class Skill
    {
        public event System.Action OnClick;

        public string Tips { get; set; }

        private readonly Transform Transform_;
        private Image Mask_;
        private GameObject BG_;
        private Text CDText_;

        private float Time_;
        private bool IsCD_;

        private readonly SkillDescriptor Desc_;

        public Skill(Transform Trans, SkillDescriptor Desc)
        {
            Transform_ = Trans;
            Time_ = Desc_.CD;
            IsCD_ = false;
            Tips = TipsHelper.Skill(Desc);

            Desc_ = Desc;

            BG_ = Transform_.Find("BG").gameObject;
            BG_.SetActive(false);
            Mask_ = Transform_.Find("Mask").GetComponent<Image>();
            CDText_ = Transform_.Find("CD").GetComponent<Text>();

            ClearCD();

            UIEventTriggerListener.Get(Transform_).AddCallback(UIEventType.Click, (Obj, Pos) =>
            {
                Use();
            });

            UIHelper.AddTips(Transform_, () => Tips);
        }

        public void Destroy()
        {
            Object.Destroy(Transform_.gameObject);
            UIEventTriggerListener.Remove(Transform_);
        }

        public void Tick(float DeltaTime)
        {
            if (PlayerManager.Mp < Desc_.Cost)
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

            Mask_.fillAmount = (1 - Time_ / Desc_.CD);
            CDText_.text = $"{Time_:0.0}s";
        }

        public void StartCD()
        {
            IsCD_ = true;
            Time_ = Desc_.CD;
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

            if (PlayerManager.Mp < Desc_.Cost)
            {
                return;
            }

            StartCD();
            PlayerManager.AddMp(-Desc_.Cost);
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
                Debug.LogError("SkillManager : null model prefab");
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

        public static Skill AddSkill(string ResName, SkillDescriptor Desc)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(SkillRoot_, false);
            Obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);
            Obj.transform.Find("Mask").GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);

            var Entity = new Skill(Obj.transform, Desc);
            SkillList_.Add(Entity);

            return Entity;
        }
    }
}