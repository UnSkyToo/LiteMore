using LiteFramework.Core.Log;
using LiteFramework.Helper;
using LiteFramework.Interface;

namespace LiteMore
{
    public class TestLogic : ILogic
    {
        public bool Startup()
        {
            AssetHelper.CacheStreamingAssets((Err, Suc, Total) =>
            {
                LLogger.LWarning($"{Err} - {Suc} - {Total}");
            });
            return true;
        }

        public void Shutdown()
        {
        }

        public void Tick(float DeltaTime)
        {
        }
    }
}