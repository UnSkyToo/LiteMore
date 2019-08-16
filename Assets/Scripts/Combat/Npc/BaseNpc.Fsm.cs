using LiteMore.Combat.Fsm;
using UnityEngine;

namespace LiteMore.Combat.Npc
{
    public partial class BaseNpc
    {
        public NpcDirection Direction { get; protected set; }
        protected readonly NpcAnimation Animation_;
        protected readonly BaseFsm Fsm_;
        private int HitSfxInterval_;

        public void PlayAnimation(string AnimName, bool IsLoop)
        {
            Animation_.Play(AnimName, IsLoop);
        }

        public bool AnimationIsEnd()
        {
            return Animation_.IsEnd();
        }

        public bool IsFsmState(FsmStateName StateName)
        {
            return Fsm_.GetStateName() == StateName;
        }

        public void SetDirection(NpcDirection Dir)
        {
            if (Direction == Dir)
            {
                return;
            }

            Direction = Dir;
            switch (Direction)
            {
                case NpcDirection.Left:
                    GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case NpcDirection.Right:
                    GetComponent<SpriteRenderer>().flipX = false;
                    break;
                default:
                    break;
            }
        }

        public void TryToPlayHitSfx(string HitSfx)
        {
            if (HitSfxInterval_ <= 0)
            {
                SfxManager.AddSfx(HitSfx, Position);
                HitSfxInterval_ = 8;
            }
        }
    }
}