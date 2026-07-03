// TOY WAR RUSH - UnitFactory.cs
// Spawns and despawns units.

using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public static UnitFactory Instance { get; private set; }

    [SerializeField] private UnitEvolutionConfig evolutionConfig;
    [SerializeField] private GameObject defaultUnitPrefab;

    public UnitEvolutionConfig EvolutionConfig => evolutionConfig;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public UnitController SpawnUnit(UnitTier tier, Vector3 localOffset = default)
    {
        var data = evolutionConfig?.GetDataForTier(tier);
        if (data == null) return null;

        var prefab = data.prefab != null ? data.prefab : defaultUnitPrefab;
        if (prefab == null) return null;

        var root = ArmyManager.Instance.FormationRoot;
        var instance = Instantiate(prefab, root);

        var unit = instance.GetComponent<UnitController>();
        if (unit == null)
        {
            Destroy(instance);
            return null;
        }

        unit.Initialize(data, localOffset);
        return unit;
    }

    public void DespawnUnit(UnitController unit)
    {
        if (unit != null)
            Destroy(unit.gameObject);
    }
}
