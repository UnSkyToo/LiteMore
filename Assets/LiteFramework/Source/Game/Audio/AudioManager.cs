using System.Collections.Generic;
using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using UnityEngine;

namespace LiteFramework.Game.Audio
{
    public static class AudioManager
    {
        private static bool MuteSound = false;
        private static bool MuteMusic = false;
        private static readonly Dictionary<uint, AudioEntity> AudioList_ = new Dictionary<uint, AudioEntity>();
        private static readonly List<AudioEntity> RemoveList_ = new List<AudioEntity>();
        private static Transform Root_ = null;

        public static bool Startup()
        {
            if (Root_ == null)
            {
                Root_ = new GameObject("Audio").transform;
                Root_.localPosition = Vector3.zero;
                Root_.localRotation = Quaternion.identity;
                Root_.localScale = Vector3.one;
            }

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

            if (Root_ != null)
            {
                Object.DestroyImmediate(Root_.gameObject);
                Root_ = null;
            }
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

        private static uint PlayAudio(AudioType Type, Transform Parent, AssetUri Uri, bool IsLoop = false, float Volume = 1.0f)
        {
            if (Uri == null)
            {
                return 0;
            }

            var Entity = new AudioEntity(Type);
            AudioList_.Add(Entity.ID, Entity);

            AssetManager.CreateAssetAsync<AudioClip>(Uri, (Clip) =>
            {
                if (Clip == null)
                {
                    LLogger.LWarning($"can't play audio : {Uri}");
                    RemoveList_.Add(Entity);
                    return;
                }

                Entity.LoadAudio(Parent, Clip, IsLoop, Volume, false);

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

        public static uint PlaySound(AssetUri Uri, bool IsLoop = false, float Volume = 1.0f)
        {
            return PlayAudio(AudioType.Sound, Root_, Uri, IsLoop, Volume);
        }

        public static uint PlayMusic(AssetUri Uri, bool IsLoop = true, float Volume = 1.0f, bool IsOnly = true)
        {
            if (IsOnly)
            {
                StopAllMusic();
            }

            return PlayAudio(AudioType.Music, Root_, Uri, IsLoop, Volume);
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