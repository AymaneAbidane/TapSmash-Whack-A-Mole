using UnityEngine;
using System.Collections;

public class IntroVidUI : MonoBehaviour
{
    [SerializeField] private float introWaitingTime = 7f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(introWaitingTime);
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
