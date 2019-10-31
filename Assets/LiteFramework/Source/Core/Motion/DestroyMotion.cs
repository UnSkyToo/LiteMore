using LiteFramework.Game.Asset;

namespace LiteFramework.Core.Motion
{
    public class DestroyMotion : BaseMotion
    {
        public DestroyMotion()
            : base()
        {
            IsEnd = true;
        }

        public override void Enter()
        {
            if (Master?.gameObject != null)
            {
                AssetManager.DeleteAsset(Master.gameObject);
            }
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}