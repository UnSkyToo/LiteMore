using System.Collections.Generic;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Game.Audio
{
    public static class AudioManager
    {
        public static Transform Root { get; set; }

        private static bool MuteSound = false;
        private static bool MuteMusic = false;
        private static readonly Dictionary<uint, AudioEntity> AudioList_ = new Dictionary<uint, AudioEntity>();
        private static readonly List<AudioEntity> RemoveList_ = new List<AudioEntity>();

        public static bool Startup()
        {
            AudioList_.Clear();
            RemoveList_.Clear();
            MuteSound = false;
            MuteMusic = false;
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in RemoveList_)
            {
                Entity.Stop();
                Entity.UnloadAudio();
            }
            RemoveList_.Clear();

            foreach (var Entity in AudioList_)
            {
                Entity.Value.Stop();
                Entity.Value.UnloadAudio();
            }
            AudioList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            foreach (var Entity in AudioList_)
            {
                if (Entity.Value.IsEnd())
                {
                    RemoveList_.Add(Entity.Value);
                }
            }

            if (RemoveList_.Count > 0)
            {
                foreach (var Entity in RemoveList_)
                {
                    AudioList_.Remove(Entity.ID);
                    Entity.UnloadAudio();
                }
                RemoveList_.Clear();
            }
        }

        public static uint PlayAudio(AudioType Type, Transform Parent, string AudioPath, bool IsLoop = false, float Volumn = 1.0f)
        {
            var Entity = new AudioEntity(Type);
            AudioList_.Add(Entity.ID, Entity);

            AssetManager.CreateAssetAsync<AudioClip>(AudioPath, (Clip) =>
            {
                if (Clip == null)
                {
                    RemoveList_.Add(Entity);
                    return;
                }

                Entity.LoadAudio(Parent, Clip, IsLoop, Volumn, false);

                switch (Type)
                {
                    case AudioType.Sound:
                        if (MuteSound)
                        {
                            Entity.Mute(MuteSound);
                        }
                        break;
                    case AudioType.Music:
                        if (MuteMusic)
                        {
                            Entity.Mute(MuteMusic);
                        }
                        break;
                    default:
                        break;
                }

                Entity.Play();
            });

            return Entity.ID;
        }

        public static uint PlaySound(string AudioPath, bool IsLoop = false, float Volume = 1.0f)
        {
            return PlayAudio(AudioType.Sound, Root, AudioPath, IsLoop, Volume);
        }

        public static uint PlayMusic(string AudioPath, bool IsLoop = true, float Volume = 1.0f, bool IsOnly = true)
        {
            if (IsOnly)
            {
                StopAllMusic();
            }

            return PlayAudio(AudioType.Music, Root, AudioPath, IsLoop, Volume);
        }

        public static void StopAudio(uint ID)
        {
            if (AudioList_.ContainsKey(ID))
            {
                AudioList_[ID].Stop();
                RemoveList_.Add(AudioList_[ID]);
            }
        }

        public static void StopAllSound()
        {
            foreach (var Entity in AudioList_)
            {
                if (Entity.Value.Type == AudioType.Sound)
                {
                    Entity.Value.Stop();
                    RemoveList_.Add(Entity.Value);
                }
            }
        }

        public static void StopAllMusic()
        {
            foreach (var Entity in AudioList_)
            {
                if (Entity.Value.Type == AudioType.Music)
                {
                    Entity.Value.Stop();
                    RemoveList_.Add(Entity.Value);
                }
            }
        }

        public static void MuteAllAudio(bool IsMute)
        {
            MuteAllSound(IsMute);
            MuteAllMusic(IsMute);
        }

        public static void MuteAllSound(bool IsMute)
        {
            MuteSound = IsMute;

            foreach(var Entity in AudioList_)
            {
                if (Entity.Value.Type == AudioType.Sound)
                {
                    Entity.Value.Mute(IsMute);
                }
            }
        }

        public static void MuteAllMusic(bool IsMute)
        {
            MuteMusic = IsMute;

            foreach (var Entity in AudioList_)
            {
                if (Entity.Value.Type == AudioType.Music)
                {
                    Entity.Value.Mute(IsMute);
                }
            }
        }
    }
}