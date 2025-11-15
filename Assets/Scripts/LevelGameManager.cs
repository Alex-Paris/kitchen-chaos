using System;
using UnityEngine;

public class LevelGameManager : MonoBehaviour
{
    public static LevelGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler<OnGamePausedChangedEventArgs> OnGamePausedChanged;
    public class OnGamePausedChangedEventArgs : EventArgs
    {
        public bool isGamePaused;
    }

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private readonly float gamePlayingTimerMax = 160f;
    private bool isGamePaused = false;

    public float CountdownToStartTimer => countdownToStartTimer;
    public float GamePlayingTimer => gamePlayingTimer;
    public float GamePlayingTimerNormalized => 1 - (gamePlayingTimer / gamePlayingTimerMax);

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnGrabAction += GameInput_OnGrabAction;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= GameInput_OnPauseAction;
        GameInput.Instance.OnGrabAction -= GameInput_OnGrabAction;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                break;

            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer >= 0) break;

                state = State.GamePlaying;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GamePlaying:
                gamePlayingTimer += Time.deltaTime;
                if (gamePlayingTimer < gamePlayingTimerMax) break;

                state = State.GameOver;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            default:
                break;
        }
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        OnGamePausedChanged?.Invoke(this, new() { isGamePaused = isGamePaused });
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }
    
    private void GameInput_OnGrabAction(object sender, EventArgs e)
    {
        if (state != State.WaitingToStart) return;
        
        state = State.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
