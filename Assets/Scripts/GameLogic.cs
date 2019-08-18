﻿using LiteFramework;
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
            var SkillParent = UIManager.FindUI<MainUI>()?.GetSkillListParent();

            SkillManager.AddMainSkill(SkillLibrary.Get(2001), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2002), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2003), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2004), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2005), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2006), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2007), PlayerManager.Master, SkillParent);
            SkillManager.AddMainSkill(SkillLibrary.Get(2008), PlayerManager.Master, SkillParent);

            NpcManager.AddNpc("boss", new Vector2(-Screen.width / 2, 0), CombatTeam.B,
                NpcManager.GenerateInitAttr(10, 1000, 0, 50, 1, 20, 20)).Scale = new Vector2(2, 2);

            SkillManager.AddPassiveSkill(SkillLibrary.Get(1005), PlayerManager.Master, -1);
        }
    }
}