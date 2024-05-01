using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    const string PLAYER_PREFS_SOUND_MUTED = "SoundMuted";
    const string PLAYER_PREFS_MUSIC_MUTED = "MusicMuted";
    const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    AudioSource audioSource;
    bool soundMuted = false;
    bool musicMuted = false;
    float volume = 0.3f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        soundMuted = PlayerPrefs.GetInt(PLAYER_PREFS_SOUND_MUTED, 0) == 1;
        musicMuted = PlayerPrefs.GetInt(PLAYER_PREFS_MUSIC_MUTED, 0) == 1;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);

        audioSource.volume = soundMuted || musicMuted ? 0 : volume;
    }
    
    public void ToggleSound()
    {
        soundMuted = !soundMuted;
        PlayerPrefs.SetInt(PLAYER_PREFS_SOUND_MUTED, soundMuted ? 1 : 0);
        PlayerPrefs.Save();

        audioSource.volume = musicMuted ? 0 : (soundMuted ? 0 : volume);
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        PlayerPrefs.SetInt(PLAYER_PREFS_MUSIC_MUTED, musicMuted ? 1 : 0);
        PlayerPrefs.Save();

        audioSource.volume = soundMuted ? 0 : (musicMuted ? 0 : volume);
    }

    public bool IsSoundMuted()
    {
        return soundMuted;
    }

    public bool IsMusicMuted()
    {
        return musicMuted;
    }

    public float GetVolume()
    {
        return volume;
    }
}
