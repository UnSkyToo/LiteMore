using System.Collections.Generic;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Shape;
using UnityEngine;

namespace LiteMore.Combat.Buff
{
    public class TriggerBuff : BaseBuff
    {
        public Vector2 Position { get; }
        public Quaternion Rotation { get; }

        private readonly Dictionary<BaseNpc, uint> NpcList_;
        private readonly List<BaseNpc> NpcDieList_;
        private readonly float Interval_;
        private readonly BaseShape Shape_;
        private readonly CombatTeam Team_;
        private float Time_;
        private int Count_;

        public TriggerBuff(BaseBuffDescriptor Desc)
            : base(BuffType.Trigger, Desc)
        {
            NpcList_ = new Dictionary<BaseNpc, uint>();
            NpcDieList_ = new List<BaseNpc>();
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
        }

        protected override void OnTrigger()
        {
            foreach (var Entity in NpcList_)
            {
                if (!Entity.Key.IsAlive)
                {
                    NpcDieList_.Add(Entity.Key);
                }
            }

            if (NpcDieList_.Count > 0)
            {
                foreach (var Entity in NpcDieList_)
                {
                    OnNpcExit(Entity, NpcList_[Entity]);
                    OnNpcDie(Entity, NpcList_[Entity]);
                    NpcList_.Remove(Entity);
                }
                NpcDieList_.Clear();
            }

            foreach (var Entity in NpcManager.GetNpcList(Team_.Opposite()))
            {
                if (!Entity.Action.CanLocked())
                {
                    continue;
                }

                if (IsAlive && Shape_.Contains(Position, Entity.Position, Rotation))
                {
                    if (!NpcList_.ContainsKey(Entity))
                    {
                        NpcList_.Add(Entity, 1);
                        OnNpcEnter(Entity, 1);
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

        private void OnNpcEnter(BaseNpc Target, uint TotalApplyCount)
        {
        }

        private void OnNpcStay(BaseNpc Target, uint TotalApplyCount)
        {
        }

        private void OnNpcExit(BaseNpc Target, uint TotalApplyCount)
        {
        }

        private void OnNpcDie(BaseNpc Target, uint TotalApplyCount)
        {
        }
    }
}