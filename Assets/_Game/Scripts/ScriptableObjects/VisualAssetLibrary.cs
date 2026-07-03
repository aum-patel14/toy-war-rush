// TOY WAR RUSH - VisualAssetLibrary.cs
// Optional visual overrides for marketplace assets.

using UnityEngine;

[CreateAssetMenu(menuName = "ToyWarRush/VisualAssetLibrary", fileName = "VisualAssetLibrary")]
public class VisualAssetLibrary : ScriptableObject
{
    [Header("Gameplay Prefab Overrides")]
    [SerializeField] private GameObject unitPrefabOverride;
    [SerializeField] private GameObject enemyUnitPrefabOverride;
    [SerializeField] private GameObject gatePrefabOverride;
    [SerializeField] private GameObject obstaclePrefabOverride;
    [SerializeField] private GameObject collectiblePrefabOverride;
    [SerializeField] private GameObject fortressPrefabOverride;
    [SerializeField] private GameObject projectilePrefabOverride;
    [SerializeField] private GameObject cannonPrefabOverride;

    [Header("Environment Materials")]
    [SerializeField] private Material laneMaterialOverride;
    [SerializeField] private Material sideMaterialOverride;

    public GameObject UnitPrefabOverride => unitPrefabOverride;
    public GameObject EnemyUnitPrefabOverride => enemyUnitPrefabOverride;
    public GameObject GatePrefabOverride => gatePrefabOverride;
    public GameObject ObstaclePrefabOverride => obstaclePrefabOverride;
    public GameObject CollectiblePrefabOverride => collectiblePrefabOverride;
    public GameObject FortressPrefabOverride => fortressPrefabOverride;
    public GameObject ProjectilePrefabOverride => projectilePrefabOverride;
    public GameObject CannonPrefabOverride => cannonPrefabOverride;
    public Material LaneMaterialOverride => laneMaterialOverride;
    public Material SideMaterialOverride => sideMaterialOverride;
}
