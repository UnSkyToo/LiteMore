using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteFramework.Game.Audio
{
    public class AudioEntity : BaseObject
    {
        public AudioType Type { get; private set; }
        public bool IsLoaded { get; private set; }
        public AudioSource Source { get; private set; }
        public GameObject Carrier { get; private set; }

        public AudioEntity(AudioType Type)
            : base()
        {
            this.Type = Type;
            this.IsLoaded = false;
        }

        public void LoadAudio(Transform Parent, AudioClip Clip, bool IsLoop, float Volume, bool IsMute)
        {
            if (IsLoaded)
            {
                return;
            }

            Carrier = new GameObject($"AudioClip_{Clip.name}_{ID}");
            Source = Carrier.AddComponent<AudioSource>();
            Source.clip = Clip;
            Source.volume = Volume;
            Source.loop = IsLoop;
            Source.pitch = 1.0f;
            Source.mute = IsMute;

            Carrier.transform.SetParent(Parent, false);
            Carrier.transform.localPosition = Vector3.zero;
            Carrier.transform.localScale = Vector3.one;
            IsLoaded = true;
        }

        public void UnloadAudio()
        {
            if (!IsLoaded)
            {
                return;
            }

            if (Carrier != null)
            {
                Source = null;
                Object.Destroy(Carrier);
                Carrier = null;
            }

            IsLoaded = false;
        }

        public bool IsValid()
        {
            return IsLoaded && Source != null;
        }

        public bool IsEnd()
        {
            if (!IsValid())
            {
                return false;
            }

            return !Source.isPlaying;
        }

        public bool Play()
        {
            if (!IsValid())
            {
                return false;
            }

            Source.Play();
            return true;
        }

        public bool Mute(bool Flag)
        {
            if (!IsValid())
            {
                return false;
            }

            Source.mute = Flag;
            return true;
        }

        public bool Stop()
        {
            if (!IsValid())
            {
                return false;
            }

            Source.Stop();
            return true;
        }

        public bool SetVolume(float Volume)
        {
            if (!IsValid())
            {
                return false;
            }

            Source.volume = Volume;
            return true;
        }
    }
}