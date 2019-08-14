using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Npc;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.Combat.Skill
{
    public static class SkillManager
    {
        private static Transform SkillRoot_;
        private static readonly List<BaseSkill> SkillList_ = new List<BaseSkill>();

        public static bool Startup()
        {
            SkillRoot_ = GameObject.Find("Skill").transform;
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

        private static Transform CreateMainSkillObject(string ResName)
        {
            var Obj = AssetManager.CreatePrefabSync("prefabs/skillicon.prefab");
            Obj.transform.SetParent(SkillRoot_, false);
            Obj.GetComponent<Image>().sprite = AssetManager.CreateAssetSync<Sprite>(ResName);
            Obj.transform.Find("Mask").GetComponent<Image>().sprite = AssetManager.CreateAssetSync<Sprite>(ResName);

            return Obj.transform;
        }

        public static MainSkill AddMainSkill(SkillDescriptor Desc)
        {
            var Obj = CreateMainSkillObject(Desc.Icon);

            var Entity = new MainSkill(Obj, Desc);
            SkillList_.Add(Entity);

            return Entity;
        }

        public static void RemoveMainSkill(MainSkill Skill)
        {
            if (Skill == null)
            {
                return;
            }

            Skill.Dispose();
            SkillList_.Remove(Skill);
        }

        public static NpcSkill AddNpcSkill(SkillDescriptor Desc, BaseNpc Master)
        {
            var Entity = new NpcSkill(Desc, Master);
            SkillList_.Add(Entity);
            return Entity;
        }

        public static PassiveSkill AddPassiveSkill(SkillDescriptor Desc, float SustainTime)
        {
            var Entity = new PassiveSkill(Desc, SustainTime);
            SkillList_.Add(Entity);
            Entity.Use(null);
            return Entity;
        }

        public static void RemovePassiveSkill(PassiveSkill Skill)
        {
            Skill.IsAlive = false;
        }

        public static uint GetSkillLevel(uint SkillID)
        {
            return 1;
        }
    }
}