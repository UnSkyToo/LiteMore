using LiteFramework;
using LiteFramework.Core.Log;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Logic;
using LiteFramework.Game.UI;
using LiteMore.Cache;
using LiteMore.Combat;
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

            if (!CombatManager.Startup()
                || !PlayerManager.Startup())
            {
                return false;
            }

            AudioManager.PlayMusic("audio/bgm_titan.mp3", true, 0.5f);

            TestSkill();
            //PlayerManager.CreateMainEmitter();
            //WaveManager.LoadWave((uint)PlayerManager.Player.Wave);

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

        private static void TestSkill()
        {
            PlayerManager.Master.AddNpcSkill(2001, false);
            PlayerManager.Master.AddNpcSkill(2002, false);
            PlayerManager.Master.AddNpcSkill(2003, false);
            PlayerManager.Master.AddNpcSkill(2004, false);
            PlayerManager.Master.AddNpcSkill(2005, false);
            PlayerManager.Master.AddNpcSkill(2006, false);
            PlayerManager.Master.AddNpcSkill(2007, false);
            PlayerManager.Master.AddNpcSkill(2008, false);

            NpcManager.AddNpc("boss", new Vector2(-Screen.width / 2, 0), CombatTeam.B,
                NpcManager.GenerateInitAttr(10, 1000, 0, 50, 1, 20, 20)).Scale = new Vector2(2, 2);

            PlayerManager.Master.AddPassiveSkill(1005, -1, true);
        }
    }
}