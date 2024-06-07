using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesUI : MonoBehaviour
{
    public event EventHandler OnClickSound;
    public static ChallengesUI Instance { get; private set; }
    [SerializeField] Button playBeginnerChallengeButton;
    [SerializeField] Button playSkilledChallengeButton;
    [SerializeField] Button playMasterChallengeButton;
    [SerializeField] Button mainMenuButton;

    [SerializeField] ChallengeDataHolder easy;
    [SerializeField] ChallengeDataHolder skilled;
    [SerializeField] ChallengeDataHolder master;

    private void Awake()
    {
        Instance = this;
        DestroyExistingDataHolder();
        playBeginnerChallengeButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            LoadChallenge(easy);
        });
        playSkilledChallengeButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            LoadChallenge(skilled);
        });
        playMasterChallengeButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            LoadChallenge(master);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        Time.timeScale = 1f;
    }

    private void DestroyExistingDataHolder()
    {
        ChallengeDataHolder existingHolder = FindObjectOfType<ChallengeDataHolder>(true);
        if (existingHolder != null)
        {
            Destroy(existingHolder.gameObject);
        }
    }

    private void LoadChallenge(ChallengeDataHolder holder)
    {
        // Instantiate new ChallengeDataHolder object
        Instantiate(holder.gameObject);

        StartCoroutine(CheckLevelToLoad());
    }


    private IEnumerator CheckLevelToLoad()
    {
        yield return new WaitForSeconds(.3f);
        Loader.Load(Loader.Scene.BeginnerChallengeScene);
    }

    private void Start()
    {
        playBeginnerChallengeButton.Select();
    }
}
