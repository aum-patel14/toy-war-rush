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
    [SerializeField] private List<ObstaclePrefabEntry> obstaclePrefabs = new();
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private GameObject fortressPrefab;

    public int CurrentLevel { get; private set; } = 1;
    public LevelData CurrentLevelData { get; private set; }
    public int StartingArmySize { get; private set; }
    public int ArmyAtLevelStart { get; private set; }
    public float LevelEndZ { get; private set; }
    public float PlayerStartZ { get; private set; }

    private readonly List<GameObject> _spawnedObjects = new();
    private FortressController _activeFortress;

    [System.Serializable]
    public class ObstaclePrefabEntry
    {
        public ObstacleType type;
        public GameObject prefab;
    }

    private GameObject GetObstaclePrefab(ObstacleType type)
    {
        foreach (var entry in obstaclePrefabs)
        {
            if (entry.type == type && entry.prefab != null)
                return entry.prefab;
        }
        return obstaclePrefab;
    }

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

        StartingArmySize = CurrentLevelData.startingArmySize + UpgradeRuntime.StartArmyBonus();
        ArmyManager.Instance?.InitializeArmy(StartingArmySize);
        ArmyAtLevelStart = ArmyManager.Instance?.ArmyCount ?? StartingArmySize;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStartZ = player.transform.position.z;
            var cam = Camera.main;
            if (cam != null)
            {
                var follow = cam.GetComponent<CameraFollow>();
                if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();
                follow.SetTarget(player.transform);
            }
        }

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

        if (levelNumber > levels.Count && levels.Count > 0)
        {
            var generated = LevelGenerator.Generate(levelNumber);
            if (generated != null) return generated;
        }

        return levels.Count > 0 ? levels[0] : null;
    }

    private void BuildLevel(LevelData data)
    {
        var root = levelRoot != null ? levelRoot : transform;
        float maxZ = 0f;

        if (gatePrefab != null)
        {
            foreach (var gate in data.gates)
            {
                var obj = Instantiate(gatePrefab, root);
                obj.transform.position = new Vector3(gate.xPosition, 0f, gate.zPosition);
                maxZ = Mathf.Max(maxZ, gate.zPosition);
                var controller = obj.GetComponent<GateController>();
                controller?.Initialize(gate.operation, gate.value);
                _spawnedObjects.Add(obj);
            }
        }

        if (obstaclePrefab != null || obstaclePrefabs.Count > 0)
        {
            foreach (var obstacle in data.obstacles)
            {
                var prefab = GetObstaclePrefab(obstacle.type);
                if (prefab == null) continue;
                var obj = Instantiate(prefab, root);
                obj.transform.position = new Vector3(obstacle.xPosition, 0f, obstacle.zPosition);
                maxZ = Mathf.Max(maxZ, obstacle.zPosition);
                var controller = obj.GetComponent<ObstacleController>();
                controller?.Initialize(obstacle.type, obstacle.unitDamage);
                _spawnedObjects.Add(obj);
            }
        }

        if (collectiblePrefab != null)
        {
            foreach (var collectible in data.collectibles)
            {
                var obj = Instantiate(collectiblePrefab, root);
                obj.transform.position = new Vector3(collectible.xPosition, 0.5f, collectible.zPosition);
                maxZ = Mathf.Max(maxZ, collectible.zPosition);
                var controller = obj.GetComponent<CollectibleController>();
                controller?.Initialize(collectible.armyBonus);
                _spawnedObjects.Add(obj);
            }
        }

        LevelEndZ = maxZ + 25f;

        if (fortressPrefab != null)
        {
            var fortress = Instantiate(fortressPrefab, root);
            fortress.transform.position = new Vector3(0f, 1.5f, LevelEndZ);
            _activeFortress = fortress.GetComponent<FortressController>();
            _activeFortress?.Initialize(data.fortressHP);
            _spawnedObjects.Add(fortress);

            if (EnemyArmyManager.Instance != null)
            {
                int midCount = Mathf.Max(0, data.fortressDefenderCount / 2);
                float midZ = LevelEndZ * 0.65f;
                EnemyArmyManager.Instance.ClearEnemies();
                EnemyArmyManager.Instance.SpawnLaneEnemies(midCount, midZ, 0f);
                EnemyArmyManager.Instance.SpawnFortressDefenders(
                    data.fortressDefenderCount - midCount,
                    fortress.transform.position);
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
        EnemyArmyManager.Instance?.ClearEnemies();
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
