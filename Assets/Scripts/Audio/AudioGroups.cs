using UnityEngine;
using Utilities.Attributes;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioGroups", menuName = "Generation/Generator Pools")]
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
