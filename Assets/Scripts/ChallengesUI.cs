using UnityEngine;
using UnityEngine.UI;

public class ChallengesUI : MonoBehaviour
{
    [SerializeField] Button playBeginnerChallengeButton;
    [SerializeField] Button playSkilledChallengeButton;
    [SerializeField] Button playMasterChallengeButton;
    [SerializeField] Button mainMenuButton;

    private void Awake()
    {

        playBeginnerChallengeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.BeginnerChallengeScene);
        });
        playSkilledChallengeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.SkilledChallengeScene);
        });
        playMasterChallengeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MasterChallengeScene);
        });
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        Time.timeScale = 1f;
    }

    private void Start()
    {
        playBeginnerChallengeButton.Select();
    }
}
