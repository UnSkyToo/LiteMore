using LiteFramework.Core.Base;

namespace LiteFramework.Game.Logic
{
    public static class LogicManager
    {
        private static readonly ListEx<ILogic> LogicList_ = new ListEx<ILogic>();

        public static bool Startup()
        {
            /*AssetManager.CreatePrefab("ui/testimage.prefab", (Obj) =>
            {
                Obj.transform.SetParent(GameObject.Find("Canvas").transform);
                Obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                var AA = AssetManager.CreateAssetWithCache<Sprite>("res1/arena.sprite", "arena_5", (Spr) =>
                {
                    if (Spr != null)
                    {
                        Obj.GetComponent<Image>().sprite = Spr;
                    }
                });
            });

            AssetManager.CreatePrefab("anim/testani.prefab", (aa) =>
            {
                if (aa != null)
                {
                    aa.transform.SetParent(GameObject.Find("Canvas-Normal").transform);
                    aa.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    aa.GetComponent<RectTransform>().localScale = Vector3.one;
                }
            });*/

            //UIManager.OpenUI<LogoUI>();

            LogicList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Logic in LogicList_)
            {
                Logic.Shutdown();
            }
            LogicList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            foreach (var Logic in LogicList_)
            {
                Logic.Tick(DeltaTime);
            }
            LogicList_.Flush();
        }

        public static void Attach(ILogic Logic)
        {
            if (Logic == null)
            {
                return;
            }

            if (Logic.Startup())
            {
                LogicList_.Add(Logic);
            }
        }

        public static void Detach(ILogic Logic)
        {
            if (Logic == null)
            {
                return;
            }

            Logic.Shutdown();
            LogicList_.Remove(Logic);
        }
    }
}