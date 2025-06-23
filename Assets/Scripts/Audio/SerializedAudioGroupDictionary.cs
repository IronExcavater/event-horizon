using System;
using Utilities.Serializables;

namespace Audio
{
    [Serializable]
    public class SerializedAudioGroupDictionary : SerializedDictionary<AudioGroupPair, string, AudioGroup> { }

    [Serializable]
    public class AudioGroupPair : KeyValuePair<string, AudioGroup> {}
}
