﻿using LiteMore.Combat.AI;
using LiteMore.Combat.AI.BehaviorTree;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public class AINpc : BaseNpc
    {
        private readonly BehaviorInputData AIData_;

        public AINpc(string Name, Transform Trans, CombatTeam Team, float[] InitAttr)
            : base(Name, Trans, Team, InitAttr)
        {
            AIData_ = new BehaviorInputData();
            AIData_.Enabled = true;
            AIData_.Attacker = this;
            AIData_.Node = AIFactory.CreateNormalAI();
            AIData_.SkillID = 0;
        }

        public override void Tick(float DeltaTime)
        {
            base.Tick(DeltaTime);

            if (!AIData_.Enabled)
            {
                return;
            }

            AIData_.TickTime = AIData_.TickTime - DeltaTime;
            if (AIData_.TickTime <= 0)
            {
                AIData_.TickTime = 0.2f;
                AIData_.DeltaTime = 0.2f;

                if (AIData_.Node.Evaluate(AIData_))
                {
                    AIData_.Node.Tick(AIData_);
                }
            }
        }

        public void EnableAI(bool Enabled)
        {
            AIData_.Enabled = Enabled;
        }
    }
}