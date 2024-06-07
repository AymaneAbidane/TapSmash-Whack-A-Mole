using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public event EventHandler OnClickSound;
    public static MainMenuUI Instance { get; private set; }
    [SerializeField] Button goToLevelsButton;
    [SerializeField] Button soundButton;
    [SerializeField] Button musicButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button tcButton;
    [SerializeField] Button ppButton;

    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

    private void Awake()
    {
        Instance = this;
        goToLevelsButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.ChallengesScene);
            OnClickSound?.Invoke(this, EventArgs.Empty);
        });
        soundButton.onClick.AddListener(() => {
            SoundManager.Instance.ToggleSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            UpdateSoundButton();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ToggleMusic();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            UpdateMusicButton();
        });
        tcButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.OpenURL("https://sites.google.com/view/");
        });
        ppButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.OpenURL("https://sites.google.com/view/");
        });
        quitButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

    private void Start()
    {
        goToLevelsButton.Select();
        quitButton.gameObject.SetActive(false);
        UpdateSoundButton();
        UpdateMusicButton();
    }

    private void UpdateSoundButton()
    {
        if (SoundManager.Instance.IsSoundMuted())
            soundButton.image.sprite = soundOffSprite;
        else
            soundButton.image.sprite = soundOnSprite;
    }

    private void UpdateMusicButton()
    {
        if (MusicManager.Instance.IsMusicMuted())
            musicButton.image.sprite = musicOffSprite;
        else
            musicButton.image.sprite = musicOnSprite;
    }
}
