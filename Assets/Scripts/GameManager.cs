using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private enum State { WaitingToStart, GamePlaying, GameOver }

    private State state;
    private HashSet<Mole> currentMoles = new HashSet<Mole>();

    private const string HIGH_SCORE_KEY = "HighScore";

    private bool isGamePaused = false;
    private float waitingToStartTimer = 1f;
    private float gamePlayingHighTimer;
    private float gamePlayingTimer = 0f;
    private bool isNewBest = false;
    private float gamePlayingTimerMax = 200f;
    private float nextMoleSpawnTime;


    [SerializeField] private List<Mole> moles;
    [SerializeField] private TMPro.TextMeshProUGUI molesPerSecondText;
    [SerializeField] private TextMeshProUGUI bestMolesPerSecondText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private float timeBetweenMoleSpawns = 1.5f;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
        resumeButton.onClick.AddListener(() =>
        {
            TogglePauseGame();
        });
    }
    private void Start()
    {

        //List<Mole> _moles = MoleManager.Instance.GetMoles() as List<Mole>;
        //moles.AddRange(_moles);

        // Find all Mole instances in the scene and add them to the moles list
        FindAllMoleInScene();

        nextMoleSpawnTime = Time.time + timeBetweenMoleSpawns;
        HideAndClearMoles();
        LoadHighScore();
    }

    void Update()
    {
        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer += Time.deltaTime;

                // Print the time with three numbers after the decimal point
                molesPerSecondText.text = gamePlayingTimer.ToString("F3");
                bestMolesPerSecondText.text = GetGamePlayingHighTimer().ToString("F3");
                //molesPerSecondText.text = Mathf.Abs(gamePlayingTimer).ToString("F3");

                // Check if it's time to start another mole
                if (Time.time >= nextMoleSpawnTime)
                {
                    // Choose a random mole
                    int index = UnityEngine.Random.Range(0, moles.Count);
                    // Doesn't matter if it's already doing something, we'll just try again next frame.
                    if (!currentMoles.Contains(moles[index]))
                    {
                        currentMoles.Add(moles[index]);
                        moles[index].Activate(1);
                        // Set the time for the next mole spawn
                        nextMoleSpawnTime = Time.time + timeBetweenMoleSpawns;
                    }
                }
                break;
            case State.GameOver:
                
                break;
        }
    }
    private void FindAllMoleInScene()
    {
        Mole[] foundMoles = FindObjectsOfType<Mole>();
        moles.AddRange(foundMoles);
    }
    private void HideAndClearMoles()
    {
        for (int i = 0; i < moles.Count; i++)
        {
            moles[i].Hide();
            moles[i].SetIndex(i);
        }
        // Remove any old game state.
        currentMoles.Clear();
    }
    private void LoadHighScore()
    {
        // Load the high score from PlayerPrefs
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY))
        {
            gamePlayingHighTimer = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        }
        else
        {
            // Set a default high score if it's the first time playing
            gamePlayingHighTimer = 0f;
        }
    }

    private void SaveHighScore()
    {
        // Save the high score to PlayerPrefs
        PlayerPrefs.SetFloat(HIGH_SCORE_KEY, gamePlayingHighTimer);
        PlayerPrefs.Save();
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
    
    public float GetGamePlayinTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
    public void GameOver()
    {
        if (gamePlayingTimer > gamePlayingHighTimer)
        {
            gamePlayingHighTimer = gamePlayingTimer;
            SaveHighScore();
            isNewBest = true;
        }
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public float GetMolesPerSecond()
    {
        return gamePlayingTimer;
    }
    public float GetGamePlayingHighTimer()
    {
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY))
        {
            gamePlayingHighTimer = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        }
        else
        {
            // Set a default high score if it's the first time playing
            gamePlayingHighTimer = 0f;
        }
        return gamePlayingHighTimer;
    }
    public bool GetIsNewBest()
    {
        return isNewBest;
    }
    public void MoleMissed(int moleIndex, bool isMole)
    {
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);

        if (isMole)
        {
            GameOver();
        }
    }
    public void MoleHited(int moleIndex)
    {
        // Remove from active moles.
        currentMoles.Remove(moles[moleIndex]);
    }
}
