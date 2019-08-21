using LiteMore.Combat.AI.BehaviorTree;

namespace LiteMore.Combat.AI
{
    public static class AIFactory
    {
        public static BehaviorNode CreateNormalAI()
        {
            var Root = BehaviorFactory.CreatePrioritySelectorNode("Root", null, null);

            var FindSkillNode = BehaviorFactory.CreatePrioritySelectorNode("FindSkill", Root, new CombatPrecondition_TryFindSkill());
            var FindSkillTargetNode = BehaviorFactory.CreatePrioritySelectorNode("FindSkillTarget", FindSkillNode, new CombatPrecondition_TryFindSkillTarget());

            var NearTargetNode = BehaviorFactory.CreatePrioritySelectorNode("NearTarget", FindSkillTargetNode, new CombatPrecondition_IsNearTarget());
            var UseSkillNode = BehaviorFactory.CreateTerminalNode<CombatTerminal_UseSkill>("UseSkill", NearTargetNode, null);

            var FarTargetNode = BehaviorFactory.CreatePrioritySelectorNode("FarTarget", FindSkillTargetNode, new CombatPrecondition_IsFarTarget());
            var MoveToTarget = BehaviorFactory.CreateTerminalNode<CombatTerminal_MoveToTarget>("MoveToTarget", FarTargetNode, null);

            return Root;
        }
    }
}