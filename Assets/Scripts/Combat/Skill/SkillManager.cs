using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Skill
{
    public static class SkillManager
    {
        private static Transform SkillRoot_;
        private static GameObject ModelPrefab_;
        private static readonly List<BaseSkill> SkillList_ = new List<BaseSkill>();

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

        private static Transform CreateSkillObject(string ResName)
        {
            var Obj = Object.Instantiate(ModelPrefab_);
            Obj.transform.SetParent(SkillRoot_, false);
            Obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);
            Obj.transform.Find("Mask").GetComponent<Image>().sprite = Resources.Load<Sprite>(ResName);

            return Obj.transform;
        }

        public static ClickSkill AddClickSkill(string ResName, SkillDescriptor Desc)
        {
            var Obj = CreateSkillObject(ResName);

            var Entity = new ClickSkill(Obj, Desc);
            SkillList_.Add(Entity);

            return Entity;
        }

        public static DragPositionSkill AddDragPositionSkill(string ResName, SkillDescriptor Desc)
        {
            var Obj = CreateSkillObject(ResName);

            var Entity = new DragPositionSkill(Obj, Desc);
            SkillList_.Add(Entity);

            return Entity;
        }

        public static DragDirectionSkill AddDragDirectionSkill(string ResName, SkillDescriptor Desc, Vector2 OriginPos)
        {
            var Obj = CreateSkillObject(ResName);

            var Entity = new DragDirectionSkill(Obj, Desc, OriginPos);
            SkillList_.Add(Entity);

            return Entity;
        }
    }
}