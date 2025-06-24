using UnityEngine;
using Utilities.Attributes;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioGroups", menuName = "Audio/AudioGroups")]
    public class AudioGroups : ScriptableObject
    {
        [SerializedDictionaryField(KeyLabel = "Key", ValueLabel = "Audio Clips")]
        public SerializedAudioGroupDictionary audioGroups = new();

        private void OnEnable()
        {
            audioGroups.Initialize();
        }
    }
}
