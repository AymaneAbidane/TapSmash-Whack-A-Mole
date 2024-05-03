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

        playBeginnerChallengeButton.onClick.AddListener(() => {
            LoadChallenge(easy);
        });
        playSkilledChallengeButton.onClick.AddListener(() => {
            LoadChallenge(skilled);

        });
        playMasterChallengeButton.onClick.AddListener(() => {
            LoadChallenge(master);

        });
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        Time.timeScale = 1f;
    }

    private void LoadChallenge(ChallengeDataHolder holder) {
        Instantiate(holder.gameObject);
        StartCoroutine(CheckLevelToLoad(holder));
    }

    private IEnumerator CheckLevelToLoad(ChallengeDataHolder holder) {
        yield return new WaitForSeconds(.3f);
        if (holder == easy) {
            Loader.Load(Loader.Scene.BeginnerChallengeScene);
        }
        else if (holder == skilled) {
            Loader.Load(Loader.Scene.SkilledChallengeScene);
        }
        else if (holder == master) {
            Loader.Load(Loader.Scene.MasterChallengeScene);
        }
    }

    private void Start()
    {
        playBeginnerChallengeButton.Select();
    }
}
