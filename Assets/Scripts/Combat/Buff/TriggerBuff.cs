using System.Collections.Generic;
using LiteFramework.Game.Asset;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Buff
{
    public class TriggerBuffDescriptor : BaseBuffDescriptor
    {
        public List<uint> BuffList { get; }
        //public BaseShape Shape { get; }
        public float Radius { get; } // only support circle shape
        public CombatTeam Team { get; }

        public TriggerBuffDescriptor(string Name, float Duration, float Interval, float WaitTime, List<uint> BuffList, float Radius, CombatTeam Team)
            : base(Name, Duration, Interval, WaitTime)
        {
            this.BuffList = BuffList;
            this.Radius = Radius;
            this.Team = Team;
        }
    }

    public class TriggerBuff : BaseBuff
    {
        private Vector2 Position_;

        public Vector2 Position
        {
            get => Position_;
            set
            {
                Position_ = value;
                if (Obj_ != null)
                {
                    Obj_.localPosition = value;
                }
            }
        }

        public Quaternion Rotation { get; set; }

        private readonly Dictionary<BaseNpc, List<BaseBuff>> NpcList_;
        private readonly List<BaseNpc> NpcDieList_;
        private readonly CircleShape Shape_;
        private readonly CombatTeam Team_;
        private readonly List<uint> BuffList_;

        private Transform Obj_;

        public TriggerBuff(TriggerBuffDescriptor Desc)
            : base(BuffType.Trigger, Desc)
        {
            NpcList_ = new Dictionary<BaseNpc, List<BaseBuff>>();
            NpcDieList_ = new List<BaseNpc>();
            Shape_ = new CircleShape(Desc.Radius);
            Team_ = Desc.Team;
            BuffList_ = Desc.BuffList;
        }

        protected override void OnAttach()
        {
            // temp sfx
            Obj_ = AssetManager.CreatePrefabSync("prefabs/bv0.prefab").transform;
            MapManager.AddToGroundLayer(Obj_);
            Obj_.localPosition = Position;
            var SR = Obj_.GetComponent<SpriteRenderer>();
            SR.color = new Color(90.0f / 255.0f, 220.0f / 255.0f, 1.0f);
            SR.size = new Vector2(Shape_.Radius * 2, Shape_.Radius * 2);
        }

        protected override void OnDetach()
        {
            AssetManager.DeleteAsset(Obj_.gameObject);
        }

        protected override void OnTrigger()
        {
            foreach (var Entity in NpcList_)
            {
                if (!Entity.Key.IsValid())
                {
                    NpcDieList_.Add(Entity.Key);
                }
            }

            if (NpcDieList_.Count > 0)
            {
                foreach (var Entity in NpcDieList_)
                {
                    OnNpcExit(Entity, NpcList_[Entity]);
                    NpcList_.Remove(Entity);
                }
                NpcDieList_.Clear();
            }

            foreach (var Entity in NpcManager.GetNpcList(Team_))
            {
                if (IsAlive && Shape_.Contains(Position, Entity.Position, Rotation))
                {
                    if (!NpcList_.ContainsKey(Entity))
                    {
                        NpcList_.Add(Entity, new List<BaseBuff>());
                        foreach (var BuffID in BuffList_)
                        {
                            var Desc = BuffLibrary.Get(BuffID);
                            if (Desc != null)
                            {
                                NpcList_[Entity].Add(BuffManager.CreateBuff(Desc));
                            }
                        }
                        OnNpcEnter(Entity, NpcList_[Entity]);
                    }
                    else
                    {
                        OnNpcStay(Entity, NpcList_[Entity]);
                    }
                }
                else
                {
                    if (NpcList_.ContainsKey(Entity))
                    {
                        OnNpcExit(Entity, NpcList_[Entity]);
                        NpcList_.Remove(Entity);
                    }
                }
            }
        }

        private void OnNpcEnter(BaseNpc Target, List<BaseBuff> BuffList)
        {
            foreach (var Buff in BuffList)
            {
                Buff.Attach();
            }
        }

        private void OnNpcStay(BaseNpc Target, List<BaseBuff> BuffList)
        {
            foreach (var Buff in BuffList)
            {
                Buff.Trigger();
            }
        }

        private void OnNpcExit(BaseNpc Target, List<BaseBuff> BuffList)
        {
            foreach (var Buff in BuffList)
            {
                Buff.Detach();
            }
        }
    }
}