using LiteMore.Extend;
using UnityEngine;

namespace LiteMore
{
    public static class GameManager
    {
        public static bool IsPause { get; set; } = false;

        private static GameObject GameOverObj_;

        public static bool Startup()
        {
            EventManager.Startup();
            SfxManager.Startup();
            MapManager.Startup();
            NpcManager.Startup();
            BulletManager.Startup();
            EmitterManager.Startup();
            SkillManager.Startup();
            PlayerManager.Startup();

            GameOverObj_ = GameObject.Find("UI").transform.Find("GameOver").gameObject;
            GameOverObj_.SetActive(false);

            UIEventTriggerListener.Get(GameOverObj_.transform.Find("BtnRestart")).AddCallback(UIEventType.Click,
                (Obj) =>
                {
                    Restart();
                });

            CreateEmitter();
            CreateSkill();

            IsPause = false;
            return true;
        }

        public static void Shutdown()
        {
            UIEventTriggerListener.Remove(GameOverObj_.transform.Find("BtnRestart"));

            PlayerManager.Shutdown();
            SkillManager.Shutdown();
            EmitterManager.Shutdown();
            BulletManager.Shutdown();
            NpcManager.Shutdown();
            MapManager.Shutdown();
            SfxManager.Shutdown();
            EventManager.Shutdown();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public static void Tick(float DeltaTime)
        {
            if (IsPause)
            {
                return;
            }

            SfxManager.Tick(DeltaTime);
            MapManager.Tick(DeltaTime);
            NpcManager.Tick(DeltaTime);
            BulletManager.Tick(DeltaTime);
            EmitterManager.Tick(DeltaTime);
            SkillManager.Tick(DeltaTime);
            PlayerManager.Tick(DeltaTime);
        }

        public static void Restart()
        {
            Shutdown();
            Startup();
        }

        public static void OnEnterBackground()
        {
            IsPause = true;
        }

        public static void OnEnterForeground()
        {
            IsPause = false;
        }

        public static void GameOver()
        {
            IsPause = true;
            GameOverObj_.SetActive(true);
            GameOverObj_.transform.SetAsLastSibling();
        }

        private static void CreateEmitter()
        {
            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 10,
                Interval = 120.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(-Screen.width / 2 + 200, 0),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(50, 100),
                HpAttr = new EmitterRandInt(20, 40),
            });

            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 3,
                Interval = 15.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(-Screen.width / 2 + 200, -Screen.height / 3.0f),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(100, 200),
                HpAttr = new EmitterRandInt(1, 5),
            });

            EmitterManager.AddEmitter(new NpcNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 3,
                Interval = 15.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(-Screen.width / 2 + 200, Screen.height / 3.0f),
                RadiusAttr = new EmitterRandFloat(40, 100),
                SpeedAttr = new EmitterRandFloat(100, 200),
                HpAttr = new EmitterRandInt(1, 5),
            });

            /*EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Screen.width / 2 - 100, Screen.height / 4.0f),
                RadiusAttr = new EmitterRandFloat(5, 30),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
            });

            EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = new Vector2(Screen.width / 2 - 100, -Screen.height / 4.0f),
                RadiusAttr = new EmitterRandFloat(5, 30),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
            });*/

            EmitterManager.AddEmitter(new BulletNormalEmitter
            {
                TriggerCount = -1,
                EmittedCount = 5,
                Interval = 5.0f / 60.0f,
                IsAlive = true,
                IsPause = false,
                Position = MapManager.BuildPosition,
                RadiusAttr = new EmitterRandFloat(0, 0),
                SpeedAttr = new EmitterRandFloat(1000, 2000),
                DamageAttr = new EmitterRandInt(1, 3),
                ResName = "Red",
            });
        }

        private static void CreateSkill()
        {
            SkillManager.AddSkill("Textures/skill1", "镭射激光", 5, 40).OnClick += () =>
            {
                BulletManager.AddLaserBullet(MapManager.BuildPosition);
            };

            SkillManager.AddSkill("Textures/skill2", "自动弹幕", 8, 30).OnClick += () =>
            {
                /*for (var Index = 0; Index < 200; ++Index)
                {
                    var Target = NpcManager.GetRandomNpc();
                    if (Target != null)
                    {
                        var bullet = BulletManager.AddBullet("Blue", new Vector2(Screen.width / 2 - 100, 0));
                        bullet.Speed = Random.Range(1000, 2000);
                        bullet.Damage = 1;
                        bullet.Attack(Target);
                    }
                }*/

                EmitterManager.AddEmitter(new BulletNormalEmitter
                {
                    TriggerCount = 10,
                    EmittedCount = 100,
                    Interval = 30.0f / 60.0f,
                    IsAlive = true,
                    IsPause = false,
                    Position = MapManager.BuildPosition,
                    RadiusAttr = new EmitterRandFloat(0, 0),
                    SpeedAttr = new EmitterRandFloat(1000, 2000),
                    DamageAttr = new EmitterRandInt(3, 5),
                    ResName = "Blue",
                });
            };

            SkillManager.AddSkill("Textures/skill3", "放马过来", 3, 10).OnClick += () =>
            {
                for (var Index = 0; Index < 100; ++Index)
                {
                    var npc = NpcManager.AddNpc(new Vector2(Random.Range(-Screen.width / 2.0f, -100), Random.Range(-Screen.height / 2, Screen.height / 2)));
                    npc.Speed = Random.Range(80, 180);
                    npc.MoveTo(new Vector2(Screen.width / 2.0f - 100, Random.Range(-100, 100)));
                }
            };

            SkillManager.AddSkill("Textures/bossair1", "天降正义", 5, 30).OnClick += () =>
            {
                var Target = NpcManager.GetRandomNpc();
                if (Target != null)
                {
                    BulletManager.AddBombBullet(Target.Position + new Vector2(0, 300));
                }
            };
        }
    }
}