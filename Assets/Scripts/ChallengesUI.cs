using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesUI : MonoBehaviour
{
    [SerializeField] Button playBeginnerChallengeButton;
    [SerializeField] Button playSkilledChallengeButton;
    [SerializeField] Button playMasterChallengeButton;
    [SerializeField] Button mainMenuButton;

    [SerializeField] ChallengeDataHolder easy;
    [SerializeField] ChallengeDataHolder skilled;
    [SerializeField] ChallengeDataHolder master;

    private void Awake()
    {
        DestroyExistingDataHolder();
        playBeginnerChallengeButton.onClick.AddListener(() =>
        {
            LoadChallenge(easy);
        });
        playSkilledChallengeButton.onClick.AddListener(() =>
        {
            LoadChallenge(skilled);
        });
        playMasterChallengeButton.onClick.AddListener(() =>
        {
            LoadChallenge(master);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
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
