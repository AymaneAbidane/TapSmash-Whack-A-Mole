using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    private bool isGamePaused = false;
    private float waitingToStartTimer = 1f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 200f;
    private float nextMoleSpawnTime;

    [SerializeField] private List<Mole> moles;
    [SerializeField] private TMPro.TextMeshProUGUI molesPerSecondText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private float timeBetweenMoleSpawns = 2f;

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
                gamePlayingTimer -= Time.deltaTime;

                // Print the time with three numbers after the decimal point
                molesPerSecondText.text = $"{gamePlayingTimer / 60f:0.000}";

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
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public string GetMolesPerSecond()
    {
        return molesPerSecondText.ToString();
    }
}
