using UnityEngine;
using System;

[Serializable]
public class MusicTuple
{
    public AudioClip audioClip;
    public int tempo;

    public MusicTuple(AudioClip audioClip, int tempo)
    {
        this.audioClip = audioClip;
        this.tempo = tempo;
    }
}