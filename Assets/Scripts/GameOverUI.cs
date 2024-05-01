using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button challengesButton;
    [SerializeField] private Button mainMenuButton;
    //[SerializeField] private TextMeshProUGUI bestMolesPerSecondText;
    [SerializeField] private TextMeshProUGUI molesPerSecondText;
    //[SerializeField] private TextMeshProUGUI newBestMolesPerSecondText;

    [SerializeField] private bool iSNewBestMolesPerSecondText = false;

    private void Awake()
    {
        tryAgainButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.BeginnerChallengeScene);
        });
        challengesButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.ChallengesScene);
        });
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }
    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            molesPerSecondText.text = GameManager.Instance.GetMolesPerSecond();
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
