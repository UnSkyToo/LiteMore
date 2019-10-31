using LiteFramework.Core.Event;
using LiteFramework.Game.Asset;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Combat;
using LiteMore.Combat.Npc;
using LiteMore.Player;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore.UI.Logic
{
    public class HeroListUI : BaseUI
    {
        private Transform ListContent_;
        private BaseNpc Current_;

        public HeroListUI()
            : base(UIDepthMode.Normal, 0)
        {
        }

        protected override void OnOpen(params object[] Params)
        {
            ListContent_ = FindChild("List/Viewport/Content");
            Current_ = null;

            EventManager.Register<NpcAddEvent>(OnNpcAddEvent);
            EventManager.Register<NpcDieEvent>(OnNpcDieEvent);

            Refresh();
        }

        protected override void OnClose()
        {
            UIHelper.RemoveAllChildren(ListContent_, true);
            EventManager.UnRegister<NpcAddEvent>(OnNpcAddEvent);
            EventManager.UnRegister<NpcDieEvent>(OnNpcDieEvent);

            UIManager.CloseUI<JoystickUI>();
            UIManager.CloseUI<MainOperatorUI>();
        }

        private void Refresh()
        {
            UIHelper.RemoveAllChildren(ListContent_, true);
            var NpcList = NpcManager.GetNpcList(CombatTeam.A);

            foreach (var Npc in NpcList)
            {
                CreateHeroItem(Npc);
            }
        }

        private void OnNpcAddEvent(NpcAddEvent Event)
        {
            if (Event.Master.Team != CombatTeam.A)
            {
                return;
            }

            Refresh();
        }

        private void OnNpcDieEvent(NpcDieEvent Event)
        {
            if (Event.Master.Team != CombatTeam.A)
            {
                return;
            }

            if (Current_ != null && Current_.ID == Event.Master.ID)
            {
                SetCurrent(null);
            }
            else
            {
                Refresh();
            }
        }

        private GameObject CreateHeroItem(BaseNpc Npc)
        {
            var Obj = AssetManager.CreatePrefabSync(new AssetUri("prefabs/heroitem.prefab"));
            Obj.transform.SetParent(ListContent_, false);

            if (Current_ != null && Current_.ID == Npc.ID)
            {
                UIHelper.SetActive(Obj.transform, "Selected", true);
            }
            else
            {
                UIHelper.SetActive(Obj.transform, "Selected", false);
            }

            UIHelper.GetComponent<Text>(Obj.transform, "Name").text = Npc.Name;

            // temp head icon
            if (Npc.ID == PlayerManager.Master.ID)
            {
                UIHelper.GetComponent<Image>(Obj.transform, "Icon").sprite =
                    AssetManager.CreateAssetSync<Sprite>(new AssetUri("textures/build.png"));
            }

            UIHelper.AddEvent(Obj.transform, () => { SetCurrent(Npc); });
            /*Obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetCurrent(Npc);
            });*/

            return Obj;
        }

        private void SetCurrent(BaseNpc Npc)
        {
            if (Current_ != null)
            {
                if (Current_.ID == Npc.ID)
                {
                    return;
                }
                else if (Current_.ID == PlayerManager.Master.ID)
                {
                    UIManager.CloseUI<MainOperatorUI>();
                }
                else
                {
                    UIManager.CloseUI<JoystickUI>();
                }

                if (Current_ is AINpc AIO)
                {
                    AIO.EnableAI(true);
                }
            }

            Current_ = Npc;
            Refresh();

            if (Npc == null)
            {
                return;
            }

            if (Current_.ID == PlayerManager.Master.ID)
            {
                UIManager.OpenUI<MainOperatorUI>();
            }
            else
            {
                var UI = UIManager.OpenUI<JoystickUI>();
                UI.BindMaster(Current_);
            }

            if (Current_ is AINpc AIN)
            {
                AIN.EnableAI(false);
                AIN.Actor.ChangeToIdleState();
            }
        }
    }
}