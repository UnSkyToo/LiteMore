namespace Lite.Framework.AppUpdate.Stage
{
    public class IdleStage : IStage
    {
        public IdleStage(Updater Updater)
            : base(Updater, StageID.Idle)
        {
        }
    }
}