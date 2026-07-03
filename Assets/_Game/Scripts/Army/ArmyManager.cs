// TOY WAR RUSH - ArmyManager.cs
// Tracks army size, triggers evolution, handles unit loss/gain.

using UnityEngine;
using System.Collections.Generic;
using System;

public class ArmyManager : MonoBehaviour
{
    public static ArmyManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private UnitEvolutionConfig evolutionConfig;
    [SerializeField] private Transform formationRoot;

    public int ArmyCount => _activeUnits.Count;
    public UnitTier CurrentTier { get; private set; } = UnitTier.ToySoldier;
    public Transform FormationRoot => formationRoot != null ? formationRoot : transform;

    private readonly List<UnitController> _activeUnits = new();

    public static event Action<int> OnArmyCountChanged;
    public static event Action<UnitTier> OnEvolutionTriggered;
    public static event Action OnArmyDefeated;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Defeat && ArmyCount <= 0)
            OnArmyDefeated?.Invoke();
    }

    public void InitializeArmy(int startingSize)
    {
        ClearArmy();
        AddUnits(startingSize);
    }

    public void ClearArmy()
    {
        foreach (var unit in _activeUnits)
        {
            if (unit != null)
                unit.Die(silent: true);
        }
        _activeUnits.Clear();
        CurrentTier = UnitTier.ToySoldier;
        OnArmyCountChanged?.Invoke(0);
    }

    public void RegisterUnit(UnitController unit)
    {
        if (!_activeUnits.Contains(unit))
        {
            _activeUnits.Add(unit);
            CheckEvolution();
            OnArmyCountChanged?.Invoke(ArmyCount);
        }
    }

    public void AddUnit(UnitController unit)
    {
        RegisterUnit(unit);
    }

    public void RemoveUnit(UnitController unit)
    {
        _activeUnits.Remove(unit);
        OnArmyCountChanged?.Invoke(ArmyCount);

        if (ArmyCount <= 0 && GameManager.Instance?.CurrentState == GameState.Playing)
        {
            OnArmyDefeated?.Invoke();
            GameManager.Instance?.SetState(GameState.Defeat);
        }
    }

    public void AddUnits(int count)
    {
        if (UnitFactory.Instance == null) return;

        for (int i = 0; i < count; i++)
            UnitFactory.Instance.SpawnUnit(CurrentTier, GetFormationOffset(i));
    }

    public void RemoveUnits(int count)
    {
        count = Mathf.Min(count, ArmyCount);
        for (int i = 0; i < count; i++)
        {
            if (_activeUnits.Count == 0) break;
            var unit = _activeUnits[_activeUnits.Count - 1];
            unit.Die();
        }
    }

    public void MultiplyArmy(float multiplier)
    {
        int toAdd = Mathf.RoundToInt(ArmyCount * (multiplier - 1f));
        AddUnits(Mathf.Max(toAdd, 1));
    }

    public void DivideArmy(float divisor)
    {
        if (divisor <= 0f) return;
        int target = Mathf.Max(1, Mathf.RoundToInt(ArmyCount / divisor));
        int toRemove = ArmyCount - target;
        if (toRemove > 0)
            RemoveUnits(toRemove);
    }

    private void CheckEvolution()
    {
        if (evolutionConfig == null) return;

        UnitTier newTier = evolutionConfig.GetTierForCount(ArmyCount);
        if (newTier != CurrentTier)
        {
            CurrentTier = newTier;
            EvolveAllUnits(newTier);
            OnEvolutionTriggered?.Invoke(newTier);
            AudioManager.Instance?.PlaySFX("unit_evolve");
            AnalyticsManager.Instance?.LogEvent("evolution_triggered", new System.Collections.Generic.Dictionary<string, object>
            {
                { "tier", newTier.ToString() },
                { "army_count", ArmyCount }
            });
        }
    }

    private void EvolveAllUnits(UnitTier tier)
    {
        foreach (var unit in _activeUnits)
            unit.Evolve(tier);
    }

    private Vector3 GetFormationOffset(int index)
    {
        float angle = index * 137.5f * Mathf.Deg2Rad;
        float radius = 0.32f + index * 0.035f;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius * 0.78f);
    }

    public int GetTotalArmyPower()
    {
        var data = evolutionConfig?.GetDataForTier(CurrentTier);
        int damage = data != null ? data.attackDamage : 5;
        return ArmyCount * damage;
    }
}
