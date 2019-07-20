using UnityEngine;

namespace LiteMore.Combat.Npc.Fsm
{
    public class NpcFsmStateDie : NpcFsmStateBase
    {
        private bool FadeOut_;
        private float FadeOutTime_;
        private float FadeOutMaxTime_;
        private readonly SpriteRenderer SpriteRenderer_;
        private Color SpriteColor_;
        private readonly CanvasGroup CanvasGroup_;

        public NpcFsmStateDie(NpcFsm Fsm)
            : base(NpcFsmStateName.Die, Fsm)
        {
            FadeOut_ = false;
            SpriteRenderer_ = Fsm.Master.GetComponent<SpriteRenderer>();
            SpriteColor_ = SpriteRenderer_.color;
            CanvasGroup_ = Fsm.Master.GetComponent<CanvasGroup>();
        }

        public override void OnEnter(NpcEvent Event)
        {
            Fsm.Master.PlayAnimation("Die", false);
        }

        public override void OnTick(float DeltaTime)
        {
            if (FadeOut_)
            {
                FadeOutTime_ += DeltaTime;
                var T = FadeOutTime_ / FadeOutMaxTime_;
                if (T >= 1.0f)
                {
                    FadeOut_ = false;
                    T = 1.0f;
                    Fsm.Master.SetDead();
                }

                SpriteColor_.a = 1 - T;
                SpriteRenderer_.color = SpriteColor_;
                CanvasGroup_.alpha = 1 - T;
                return;
            }

            if (Fsm.Master.AnimationIsEnd())
            {
                Fsm.Master.GetComponent<Animator>().speed = 0;
                FadeOut_ = true;
                FadeOutTime_ = 0;
                FadeOutMaxTime_ = 0.5f;
                return;
            }
        }
    }
}