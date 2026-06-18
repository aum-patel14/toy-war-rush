// TOY WAR RUSH - LevelManager.cs
// Loads levels, tracks progress, handles win/lose.

using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelData> levels = new();
    [SerializeField] private Transform levelRoot;
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private GameObject obstaclePrefab;

    public int CurrentLevel { get; private set; } = 1;
    public LevelData CurrentLevelData { get; private set; }
    public int StartingArmySize { get; private set; }
    public int ArmyAtLevelStart { get; private set; }

    private readonly List<GameObject> _spawnedObjects = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        EventBus.Subscribe(GameEvents.FortressDestroyed, OnFortressDestroyed);
        ArmyManager.OnArmyDefeated += OnArmyDefeated;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameEvents.FortressDestroyed, OnFortressDestroyed);
        ArmyManager.OnArmyDefeated -= OnArmyDefeated;
    }

    public void LoadLevel(int levelNumber)
    {
        CurrentLevel = levelNumber;
        CurrentLevelData = GetLevelData(levelNumber);

        if (CurrentLevelData == null)
        {
            Debug.LogWarning($"Level {levelNumber} not found.");
            return;
        }

        ClearLevel();
        BuildLevel(CurrentLevelData);

        StartingArmySize = CurrentLevelData.startingArmySize;
        ArmyManager.Instance?.InitializeArmy(StartingArmySize);
        ArmyAtLevelStart = ArmyManager.Instance?.ArmyCount ?? StartingArmySize;

        GameManager.Instance?.SetState(GameState.Playing);
        AnalyticsManager.Instance?.LogLevelStart(levelNumber);
        EventBus.Publish(GameEvents.LevelLoaded, levelNumber);
    }

    private LevelData GetLevelData(int levelNumber)
    {
        foreach (var level in levels)
        {
            if (level != null && level.levelNumber == levelNumber)
                return level;
        }
        return levels.Count > 0 ? levels[0] : null;
    }

    private void BuildLevel(LevelData data)
    {
        var root = levelRoot != null ? levelRoot : transform;

        if (gatePrefab != null)
        {
            foreach (var gate in data.gates)
            {
                var obj = Instantiate(gatePrefab, root);
                obj.transform.position = new Vector3(gate.xPosition, 0f, gate.zPosition);
                var controller = obj.GetComponent<GateController>();
                controller?.Initialize(gate.operation, gate.value);
                _spawnedObjects.Add(obj);
            }
        }

        if (obstaclePrefab != null)
        {
            foreach (var obstacle in data.obstacles)
            {
                var obj = Instantiate(obstaclePrefab, root);
                obj.transform.position = new Vector3(obstacle.xPosition, 0f, obstacle.zPosition);
                var controller = obj.GetComponent<ObstacleController>();
                controller?.Initialize(obstacle.type, obstacle.unitDamage);
                _spawnedObjects.Add(obj);
            }
        }
    }

    private void ClearLevel()
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        _spawnedObjects.Clear();
        ArmyManager.Instance?.ClearArmy();
    }

    private void OnFortressDestroyed()
    {
        int stars = CalculateStars();
        SaveManager.Instance?.SetLevelStars(CurrentLevel, stars);
        SaveManager.Instance?.AdvanceLevel(CurrentLevel);
        GameManager.Instance?.SetState(GameState.Victory);
    }

    private void OnArmyDefeated()
    {
        if (GameManager.Instance?.CurrentState == GameState.Playing)
        {
            AnalyticsManager.Instance?.LogLevelFail(CurrentLevel);
            GameManager.Instance.SetState(GameState.Defeat);
        }
    }

    public int CalculateStars()
    {
        if (CurrentLevelData == null || ArmyAtLevelStart <= 0) return 1;

        int armyNow = ArmyManager.Instance?.ArmyCount ?? 0;
        int percentRemaining = Mathf.RoundToInt((float)armyNow / ArmyAtLevelStart * 100f);

        if (percentRemaining >= CurrentLevelData.threeStarRequirement) return 3;
        if (percentRemaining >= CurrentLevelData.twoStarRequirement) return 2;
        return 1;
    }

    public void RetryLevel() => LoadLevel(CurrentLevel);

    public void LoadNextLevel() => LoadLevel(CurrentLevel + 1);
}
