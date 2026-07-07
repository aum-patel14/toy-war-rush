// TOY WAR RUSH - GameManager.cs
// Central state machine for all game states.
// Attach to: [GameManager] GameObject in Gameplay scene.

using UnityEngine;
using System;

public enum GameState { Boot, MainMenu, Playing, Paused, Victory, Defeat }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        SetState(GameState.Boot);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        HandleStateChange(newState);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Victory:
                AnalyticsManager.Instance?.LogLevelComplete(LevelManager.Instance?.CurrentLevel ?? 1);
                RewardSystem.Instance?.GrantLevelReward();
                AdManager.Instance?.OnLevelComplete();
                break;
            case GameState.Defeat:
                AdManager.Instance?.ShowContinueOffer();
                break;
        }
    }
}
