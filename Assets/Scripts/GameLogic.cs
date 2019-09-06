using LiteFramework;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Core.Log;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Lua;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteFramework.Interface;
using LiteMore.Cache;
using LiteMore.Combat;
using LiteMore.Combat.AI.Filter;
using LiteMore.Combat.Bullet;
using LiteMore.Combat.Npc;
using LiteMore.Combat.Skill;
using LiteMore.Combat.Wave;
using LiteMore.Data;
using LiteMore.Player;
using LiteMore.UI;
using LiteMore.UI.Logic;
using UnityEngine;

namespace LiteMore
{
    public class GameLogic : ILogic
    {
        public GameLogic()
        {
        }

        public bool Startup()
        {
            Input.multiTouchEnabled = true;

            LocalCache.LoadCache();
            LocalData.Generate();
            Lang.Load();

            AudioManager.Root = Configure.AudioRoot;
            AudioManager.MuteAllAudio(true);

            foreach (var Entity in UIConfigure.UIList)
            {
                LiteConfigure.UIDescList.Add(Entity.Key, Entity.Value);
            }

            if (!CombatManager.Startup() || !PlayerManager.Startup())
            {
                return false;
            }

            AudioManager.PlayMusic("audio/bgm_titan.mp3", true, 0.5f);

            TestSkill();
            //PlayerManager.CreateMainEmitter();
            //WaveManager.LoadWave((uint)PlayerManager.Player.Wave);

            LuaRuntime.ExecuteMainLuaFile("main");
            return true;
        }

        public void Shutdown()
        {
            PlayerManager.Shutdown();
            CombatManager.Shutdown();

            PlayerManager.SaveToArchive();
            LocalCache.SaveCache();
        }

        public void Tick(float DeltaTime)
        {
            CombatManager.Tick(DeltaTime);
            PlayerManager.Tick(DeltaTime);
        }

        public void OnGUI()
        {
        }

        private void TestSkill()
        {
            PlayerManager.Master.Skill.AddNpcSkill(2001);
            PlayerManager.Master.Skill.AddNpcSkill(2002);
            PlayerManager.Master.Skill.AddNpcSkill(2003);
            PlayerManager.Master.Skill.AddNpcSkill(2004);
            PlayerManager.Master.Skill.AddNpcSkill(2005);
            PlayerManager.Master.Skill.AddNpcSkill(2006);
            PlayerManager.Master.Skill.AddNpcSkill(2007);
            PlayerManager.Master.Skill.AddNpcSkill(2012);
            PlayerManager.Master.Skill.AddPassiveSkill(3005, -1, true);

            //NpcManager.AddNpc("boss", new Vector2(-Screen.width / 2.0f, 0), CombatTeam.B, 3, NpcManager.GenerateInitAttr(50, 1000, 10, 0, 0, 5, 1, 30, 30));

            /*for (var Index = 0; Index < 10; ++Index)
            {
                var Npc = NpcManager.AddNpc($"Archer{Index + 1}",
                    new Vector2(Screen.width / 2.0f - 100, Mathf.Lerp(-Screen.height / 2.0f + 50, Screen.height / 2.0f - 50, (float)Index / 9.0f)), CombatTeam.A, 2,
                    NpcManager.GenerateInitAttr(200, 200, 0, 100, 10, 1, 0, 1500, 30));
                Npc.Skill.AddNpcSkill(2011); // 箭雨-主动
            }*/

            var Archer = NpcManager.AddNpc("Archer", new Vector2(Screen.width / 2 - 100, 0), CombatTeam.A, 1,
                NpcManager.GenerateInitAttr(200, 100, 0, 100, 10, 1, 0, 30, 30));
            Archer.EnableAI(false);

            var UI = UIManager.OpenUI<JoystickUI>();
            UI.BindMaster(Archer);
        }
    }
}