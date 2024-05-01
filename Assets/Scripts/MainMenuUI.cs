using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
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
        goToLevelsButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.ChallengesScene);
        });
        soundButton.onClick.AddListener(() => {
            MusicManager.Instance.ToggleSound();
            UpdateSoundButton();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ToggleMusic();
            UpdateMusicButton();
        });
        tcButton.onClick.AddListener(() => {
            Application.OpenURL("https://sites.google.com/view/");
        });
        ppButton.onClick.AddListener(() => {
            Application.OpenURL("https://sites.google.com/view/");
        });
        quitButton.onClick.AddListener(() => {
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
        if (MusicManager.Instance.IsSoundMuted())
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
