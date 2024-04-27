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

    // Hardcoded variables you may want to tune.
    private float startingTime = 30f;

    // Global variables
    private float timeRemaining;
    private HashSet<MoleManager> currentMoles = new HashSet<MoleManager>();
    private int score;
    private bool playing = false;


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
        // Show the message.
        if (type == 0)
        {
            gameOverUI.SetActive(true);
        }
        else
        {
            //bombText.SetActive(true);
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

    public void AddScore(int moleIndex)
    {
        // Add and update score.
        score += 1;
        scoreText.text = $"{score}";
        // Increase time by a little bit.
        timeRemaining += 1;
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);
    }

    public void Missed(int moleIndex, bool isMole)
    {
        if (isMole)
        {
            // Decrease time by a little bit.
            timeRemaining -= 2;
        }
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);
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
