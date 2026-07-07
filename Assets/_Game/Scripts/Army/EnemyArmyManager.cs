// TOY WAR RUSH - EnemyArmyManager.cs
// Spawns red enemy units for mid-lane and fortress defense.

using UnityEngine;
using System.Collections.Generic;

public class EnemyArmyManager : MonoBehaviour
{
    public static EnemyArmyManager Instance { get; private set; }

    [SerializeField] private GameObject enemyUnitPrefab;
    [SerializeField] private Transform enemyRoot;

    private readonly List<GameObject> _spawned = new();

    public int ActiveCount
    {
        get
        {
            _spawned.RemoveAll(go => go == null);
            return _spawned.Count;
        }
    }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ClearEnemies()
    {
        foreach (var go in _spawned)
        {
            if (go != null)
                Destroy(go);
        }
        _spawned.Clear();
    }

    public void SpawnLaneEnemies(int count, float z, float xSpread)
    {
        if (enemyUnitPrefab == null || enemyRoot == null) return;

        for (int i = 0; i < count; i++)
        {
            float ox = (i % 4 - 1.5f) * 0.6f;
            float oz = (i / 4) * 0.5f;
            var pos = new Vector3(xSpread + ox, 0.5f, z + oz);
            var go = Instantiate(enemyUnitPrefab, pos, Quaternion.identity, enemyRoot);
            _spawned.Add(go);
        }
    }

    public void SpawnFortressDefenders(int count, Vector3 fortressPos)
    {
        SpawnLaneEnemies(count, fortressPos.z - 2f, fortressPos.x);
    }

    public void RemoveEnemies(int count)
    {
        _spawned.RemoveAll(go => go == null);
        count = Mathf.Min(count, _spawned.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = _spawned.Count - 1;
            if (idx < 0) break;
            var go = _spawned[idx];
            _spawned.RemoveAt(idx);
            if (go != null)
            {
                FXManager.Instance?.PlayEffect("ObstacleHit", go.transform.position);
                Destroy(go);
            }
        }
    }
}
