using System.Collections.Generic;
using LiteFramework.Core.Event;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Npc;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Skill
{
    public static class SkillManager
    {
        private static readonly List<BaseSkill> SkillList_ = new List<BaseSkill>();

        public static bool Startup()
        {
            SkillList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in SkillList_)
            {
                Entity.Dispose();
            }
            SkillList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = SkillList_.Count - 1; Index >= 0; --Index)
            {
                SkillList_[Index].Tick(DeltaTime);

                if (!SkillList_[Index].IsAlive)
                {
                    SkillList_[Index].Dispose();
                    SkillList_.RemoveAt(Index);
                }
            }
        }

        public static BaseSkill FindSkill(uint SkillID)
        {
            foreach (var Skill in SkillList_)
            {
                if (Skill.SkillID == SkillID)
                {
                    return Skill;
                }
            }

            return null;
        }

        public static BaseSkill FindSkillWithID(uint ID)
        {
            foreach (var Skill in SkillList_)
            {
                if (Skill.ID == ID)
                {
                    return Skill;
                }
            }

            return null;
        }

        private static Transform CreateMainSkillObject(Transform Parent, string ResName)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/skillicon.prefab");
            Obj.transform.SetParent(Parent, false);
            Obj.GetComponent<Image>().sprite = AssetManager.CreateAssetSync<Sprite>(ResName);
            Obj.transform.Find("Mask").GetComponent<Image>().sprite = AssetManager.CreateAssetSync<Sprite>(ResName);

            return Obj.transform;
        }

        private static void AddSkill(BaseSkill Skill)
        {
            SkillList_.Add(Skill);
        }

        public static void RemoveSkill(BaseSkill Skill)
        {
            if (Skill == null)
            {
                return;
            }

            Skill.Dispose();
            SkillList_.Remove(Skill);
        }

        public static bool CanUseSkill(uint SkillID)
        {
            var Skill = FindSkill(SkillID);
            if (Skill == null)
            {
                return false;
            }

            return Skill.CanUse();
        }

        public static MainSkill AddMainSkill(SkillDescriptor Desc, BaseNpc Master, Transform Parent)
        {
            var Obj = CreateMainSkillObject(Parent, Desc.Icon);

            var Entity = new MainSkill(Obj, Desc, Master);
            AddSkill(Entity);
            return Entity;
        }

        public static NpcSkill AddNpcSkill(SkillDescriptor Desc, BaseNpc Master)
        {
            var Entity = new NpcSkill(Desc, Master);
            AddSkill(Entity);
            return Entity;
        }

        public static PassiveSkill AddPassiveSkill(SkillDescriptor Desc, BaseNpc Master, float SustainTime)
        {
            var Entity = new PassiveSkill(Desc, Master, SustainTime);
            AddSkill(Entity);
            Entity.Use(null);
            return Entity;
        }
    }
}