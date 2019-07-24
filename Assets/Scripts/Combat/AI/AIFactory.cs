using LiteMore.Combat.AI.BehaviorTree;

namespace LiteMore.Combat.AI
{
    public static class AIFactory
    {
        public static BehaviorNode CreateNormalAI()
        {
            var Root = BehaviorFactory.CreatePrioritySelectorNode("Root", null, null);

            /*local pIsForceMove = BehaviorFactory:createPrioritySelectorNode('IsForceMove', pRoot, BehaviorPrecondition_IsForceMove: new ())

            local pForceMove = BehaviorFactory:createTerminalNode('ForceMove', pIsForceMove, nil, Behavior_ForceMove: new ())


            local pSkill = BehaviorFactory:createPrioritySelectorNode('FindSkill', pRoot, BehaviorPrecondition_FindSkill: new ())

            local pFindSkillTarget = BehaviorFactory:createPrioritySelectorNode('FindSkillTarget', pSkill, BehaviorPrecondition_FindSkillTarget: new ())


            local pNearTarget = BehaviorFactory:createPrioritySelectorNode('NearTarget', pFindSkillTarget, BehaviorPrecondition_IsNearTarget: new ())

            local pUseSkill = BehaviorFactory:createTerminalNode('UseSkill', pNearTarget, nil, Behavior_UseSkill: new ())


            local pFarTarget = BehaviorFactory:createPrioritySelectorNode('FarTarget', pFindSkillTarget, BehaviorPrecondition_IsFarTarget: new ())

            local pMoveToTarget = BehaviorFactory:createTerminalNode('MoveToTarget', pFarTarget, nil, Behavior_MoveToTarget: new ())


            return pRoot*/

            return null;
        }
    }
}