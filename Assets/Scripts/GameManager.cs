using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<MoleManager> moles;

    [Header("UI")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject scoreAndTimeUI;
    [SerializeField] private GameObject gameOverUI;
    //[SerializeField] private GameObject bombText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreFloatingTextPrefab;
    [SerializeField] private GameObject timeFloatingTextPrefab;

    // Hardcoded variables you may want to tune.
    private float startingTime = 60f;

    // Global variables
    private float timeRemaining;
    [SerializeField] private HashSet<MoleManager> currentMoles = new();
    private int score;
    private bool playing = false;

    private Coroutine gameOverCoroutine;

    private void Start()
    {
        // Clear the list to ensure it's empty before adding objects
        moles.Clear();

        // Find all objects of type MoleManager in the scene and add them to the list
        MoleManager[] foundMoles = FindObjectsOfType<MoleManager>();
        moles.AddRange(foundMoles);

        //foreach (MoleManager mole in moles)
        //{
        //}
        gameOverUI.SetActive(false);
        scoreAndTimeUI.SetActive(false);
    }
    public void StartGame()
    {
        // Hide/show the UI elements we don't/do want to see.
        playButton.SetActive(false);
        gameOverUI.SetActive(false);
        //bombText.SetActive(false);
        scoreAndTimeUI.SetActive(true);
        // Hide all the visible moles.
        for (int i = 0; i < moles.Count; i++)
        {
            moles[i].Hide();
            moles[i].SetIndex(i);
        }
        // Remove any old game state.
        currentMoles.Clear();
        // Start with 30 seconds.
        timeRemaining = startingTime;
        score = 0;
        scoreText.text = "0";
        playing = true;
    }

    public void GameOver(int type)
    {
        // Stop the previous coroutine if it's running
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }

        // Show the message.
        if (type == 0)
        {
            gameOverUI.SetActive(true);
            // Start the coroutine to deactivate gameOverUI after 1.5 seconds
            gameOverCoroutine = StartCoroutine(DeactivateGameOverUI());
        }
        else
        {
            
        }

        // Hide all moles.
        foreach (MoleManager mole in moles)
        {
            mole.StopGame();
        }

        // Stop the game and show the start UI.
        playing = false;
        playButton.SetActive(true);
    }

    private IEnumerator DeactivateGameOverUI()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds
        gameOverUI.SetActive(false); // Deactivate the game over UI
        GameOver(1); // Call GameOver with type 1
    }

    public void AddScore(int moleIndex, int scoreValue)
    {
        // Add and update score.
        score += scoreValue;
        scoreText.text = $"{score}";
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);
    }

    public void Missed(int moleIndex, bool isMole)
    {
        // Decrease time if it's a mole.
        if (isMole)
        {
            timeRemaining -= 2;
        }
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);
    }

    public void ShowScoreFloatingText(string text, Vector2 position)
    {
        if (scoreFloatingTextPrefab)
        {
            GameObject prefab = Instantiate(scoreFloatingTextPrefab, position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }
    public void ShowTimeFloatingText(string text, Vector2 position)
    {
        if (timeFloatingTextPrefab)
        {
            GameObject prefab = Instantiate(timeFloatingTextPrefab, position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }

    void Update()
    {
        if (playing)
        {
            // Update time.
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                GameOver(0);
            }
            timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
            // Check if we need to start any more moles.
            if (currentMoles.Count <= (score / 10))
            {
                // Choose a random mole.
                int index = Random.Range(0, moles.Count);
                // Doesn't matter if it's already doing something, we'll just try again next frame.
                if (!currentMoles.Contains(moles[index]))
                {
                    currentMoles.Add(moles[index]);
                    moles[index].Activate(score / 10);
                }
            }
        }
    }
}
