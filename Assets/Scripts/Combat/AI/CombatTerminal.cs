using LiteMore.Combat.AI.BehaviorTree;

namespace LiteMore.Combat.AI
{
    public class Behavior_MoveToTarget : BehaviorTerminalNode
    {
        public Behavior_MoveToTarget()
            : base()
        {
        }

        protected override void OnEnter(BehaviorInputData Input)
        {
            /*Role target = RoleManager.GetInstance().FindRole(input.TargetId);

            if (target != null)
            {
                MoveEvent moveEvent = new MoveEvent();
                moveEvent.RoleId = input.AttackerId;
                moveEvent.Position = target.Position;

                GameEventManager.GetInstance().SendGameEvent(moveEvent);
            }*/
        }
    }
}