using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{

    const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    const string PLAYER_PREFS_SOUND_MUTED = "SoundMuted";

    public static SoundManager Instance { get; private set; }
    
    [SerializeField] AudioClipRefsSO audioClipRefsSO;
     [SerializeField] private AudioSource audioSource;

    private bool soundMuted = false;

    private float volume = 1f;

    private void Awake() {
        Instance = this;
    
        soundMuted = PlayerPrefs.GetInt(PLAYER_PREFS_SOUND_MUTED, 0) == 1;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);

        // audioSource.volume = soundMuted ? 0 : volume;
    }
    private void Start()
    {
        if (Mole.Instance != null)
        {
            Mole.Instance.OnMoleHided += Mole_OnMoleHided;
            Mole.Instance.OnMoleHited += Mole_OnMoleHited;
            Mole.Instance.OnMoleMissed += Mole_OnMoleMissed;
            Mole.Instance.OnMoleShowed += Mole_OnMoleShowed;
        }

        if (MainMenuUI.Instance != null)
        {
            MainMenuUI.Instance.OnClickSound += MainMenuUI_OnClickSound;
        }

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.OnClickSound += GameOverUI_OnClickSound;
        }

        if (GamePauseUI.Instance != null)
        {
            GamePauseUI.Instance.OnClickSound += GamePauseUI_OnClickSound;
        }

        if (ChallengesUI.Instance != null)
        {
            ChallengesUI.Instance.OnClickSound += ChallengesUI_OnClickSound;
        }
    }

    private void Mole_OnMoleHided(object sender, System.EventArgs e)
    {
        // PlaySound(audioClipRefsSO.moleHided, Camera.main.transform.position);
        audioSource.PlayOneShot(audioClipRefsSO.moleHided);

        Debug.Log("Mole_OnMoleHided");
    }

    private void Mole_OnMoleHited(object sender, System.EventArgs e)
    {
        Debug.Log("Mole_OnMoleHited");
        PlaySound(audioClipRefsSO.moleHit, Camera.main.transform.position);
    }

    private void Mole_OnMoleMissed(object sender, System.EventArgs e)
    {
        Debug.Log("Mole_OnMoleMissed");
        PlaySound(audioClipRefsSO.moleMiss, Camera.main.transform.position);
    }

    private void Mole_OnMoleShowed(object sender, System.EventArgs e)
    {
        Debug.Log("Mole_OnMoleShowed");
        audioSource.PlayOneShot(audioClipRefsSO.moleShow);
        // PlaySound(audioClipRefsSO.moleShow, Camera.main.transform.position);
    }
    private void MainMenuUI_OnClickSound(object sender, System.EventArgs e)
    {
        Debug.Log("MainMenuUI_OnClickSound");
        PlaySound(audioClipRefsSO.click, Camera.main.transform.position);
    }
    private void GameOverUI_OnClickSound(object sender, System.EventArgs e)
    {
        Debug.Log("GameOverUI_OnClickSound");
        PlaySound(audioClipRefsSO.click, Camera.main.transform.position);
    }
    private void GamePauseUI_OnClickSound(object sender, System.EventArgs e)
    {
        Debug.Log("GamePauseUI_OnClickSound");
        PlaySound(audioClipRefsSO.click, Camera.main.transform.position);
    }
    private void ChallengesUI_OnClickSound(object sender, System.EventArgs e)
    {
        Debug.Log("ChallengesUI_OnClickSound");
        PlaySound(audioClipRefsSO.click, Camera.main.transform.position);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1.1f)
    {
       AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }
    
    public void ToggleSound()
    {
        soundMuted = !soundMuted;
        PlayerPrefs.SetInt(PLAYER_PREFS_SOUND_MUTED, soundMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        volume = soundMuted ? 0f : 1f;
        // audioSource.volume = soundMuted ? 0 : volume;
    }
    public float GetVolume() {
       return volume;
    }
    public bool IsSoundMuted()
    {
        return soundMuted;
    }
    //public void PlayFootStepsSound(Vector3 position, float volume) { 
    //    PlaySound(audioClipRefsSO.footStep,position,volume);
    //}
    //public void ChangeVolume() {
    //    volume += .1f;
    //    if (volume > 1f) { 
    //        volume = 0f;
    //    }

    //    PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
    //    PlayerPrefs.Save();
    //}
}
