using UnityEngine;
using UnityEngine.Audio;
using Utilities;

namespace Audio
{
    [DoNotDestroySingleton]
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer audioMixer;

        [Header("Audio Clips:")]
        [SerializeField] private AudioGroups _audioGroups;

        public static AudioGroups AudioGroups => Instance._audioGroups;
    }

}
