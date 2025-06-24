using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Audio { get; private set; }
    
    public AudioMixer audioMixer;
    
    [Header("Audio Sources:")]
    [SerializeField] private AudioSource[] musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips:")]
    public MusicTuple musicExplorerSpace;
    public MusicTuple musicTitan, musicDreams;

    [Header("SFX Clips:")]
    public AudioClip grapplingHook;
    public AudioClip explosion, click, powerDown;
    
    private MusicTuple currentMusic;
    public List<MusicTuple> loopMusic = new();
    private int sourceToggle;
    private double endDspTime;

    private void Awake()
    {
        if (Audio == null)
        {
            Audio = this;
            DontDestroyOnLoad(Audio);
        }
        else Destroy(gameObject);
    }

    
    private void Update()
    {
        LoopMusic();
    }

    private void LoopMusic()
    {
        if (AudioSettings.dspTime < endDspTime - 1 || loopMusic.Count == 0) return;
        
        currentMusic = GetNextMusicInLoop();
        ScheduleMusicClip(currentMusic, endDspTime);
    }
    
    public static bool IsCurrentClip(AudioClip clip)
    {
        var currentClip = GetCurrentClip();
        return currentClip != null && currentClip.Equals(clip);
    }

    public static AudioClip GetCurrentClip() { return Audio.musicSource[1 - Audio.sourceToggle].clip; }

    public static void PlayMusicOneShotNextBar(MusicTuple clip) { Audio.ScheduleMusicClip(clip, Audio.NextBar()); }
    public static void PlayMusicOneShotNextBeat(MusicTuple clip) { Audio.ScheduleMusicClip(clip, Audio.NextBeat()); }
    public static void PlayMusicOneShotNow(MusicTuple clip) { Audio.ScheduleMusicClip(clip, AudioSettings.dspTime); }
    
    public static void PlayMusicLoopNextBar(MusicTuple clip) { Audio.PlayMusicLoop(clip, Audio.NextBar()); }
    public static void PlayMusicLoopNextBeat(MusicTuple clip) { Audio.PlayMusicLoop(clip, Audio.NextBeat()); }
    public static void PlayMusicLoopNow(MusicTuple clip) { Audio.PlayMusicLoop(clip, AudioSettings.dspTime); }


    void Start()
    {
        AddMusicToLoop(musicDreams);
        AddMusicToLoop(musicExplorerSpace);
        AddMusicToLoop(musicTitan);
    }   
    public static void AddMusicToLoop(MusicTuple clip, int loopIndex = -1)
    {
        if (loopIndex == -1) Audio.loopMusic.Add(clip);
        else Audio.loopMusic.Insert(loopIndex, clip);

        if (Audio.currentMusic != null) return;
        Audio.ScheduleMusicClip(clip, AudioSettings.dspTime);
        Audio.currentMusic = clip;
    }
    
    private static MusicTuple GetNextMusicInLoop()
    {
        var nextIndex = Audio.currentMusic == null ? 0 : Audio.loopMusic.IndexOf(Audio.currentMusic) + 1;
        return Audio.loopMusic[nextIndex >= Audio.loopMusic.Count ? 0 : nextIndex];
    }

    private void PlayMusicLoop(MusicTuple clip, double nextDspTime)
    {
        if (IsCurrentClip(clip.audioClip))
            return;

        ScheduleMusicClip(clip, nextDspTime);
        AddMusicToLoop(clip, Audio.currentMusic == null ? 0 : loopMusic.IndexOf(Audio.currentMusic) + 1);
        currentMusic = clip;
    }
    
    public static void PlaySfxOneShot(AudioClip clip)
    {
        Audio.sfxSource.PlayOneShot(clip);
    }

    private double NextBar()
    {
        if (currentMusic == null) return AudioSettings.dspTime;
        // Calculate next musical bar time of current clip
        var barDuration = 60d / currentMusic.tempo * 4; // minute / bpm * time signature (assumed 4/4)
        var clipElapsedDspTime = (double) musicSource[1 - sourceToggle].timeSamples / currentMusic.audioClip.frequency;
        return AudioSettings.dspTime + barDuration - clipElapsedDspTime % barDuration;
    }

    private double NextBeat()
    {
        if (currentMusic == null) return AudioSettings.dspTime;
        // Calculate next musical beat time of current clip
        var beatDuration = 60d / currentMusic.tempo;
        var clipElapsedDspTime = (double) musicSource[1 - sourceToggle].timeSamples / currentMusic.audioClip.frequency;
        return AudioSettings.dspTime + beatDuration - clipElapsedDspTime % beatDuration;
    }

    private void ScheduleMusicClip(MusicTuple clip, double nextDspTime)
    {
        if (IsCurrentClip(clip.audioClip)) return;
        // Load next clip and schedule at next time
        musicSource[sourceToggle].clip = clip.audioClip;
        musicSource[sourceToggle].PlayScheduled(nextDspTime);
        // Set end time to next time plus clip's duration
        EndCurrentClip(nextDspTime);
        endDspTime = nextDspTime + (double) clip.audioClip.samples / clip.audioClip.frequency;
        // Switch source toggle
        sourceToggle = 1 - sourceToggle;
    }

    private void EndCurrentClip(double nextDspTime)
    {
        if (GetCurrentClip() != null) musicSource[1 - sourceToggle].SetScheduledEndTime(nextDspTime);
    }
}