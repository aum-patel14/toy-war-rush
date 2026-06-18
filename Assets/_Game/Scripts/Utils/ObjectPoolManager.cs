// TOY WAR RUSH - ObjectPoolManager.cs
// Central registry for all game object pools.

using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public string poolId;
        public GameObject prefab;
        public int initialSize = 20;
    }

    [SerializeField] private PoolConfig[] pools;

    private readonly Dictionary<string, GenericPool> _poolRegistry = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var config in pools)
        {
            if (config.prefab != null)
                _poolRegistry[config.poolId] = new GenericPool(config.prefab, config.initialSize, transform);
        }
    }

    public GameObject Spawn(string poolId, Vector3 position, Quaternion rotation)
    {
        return _poolRegistry.TryGetValue(poolId, out var pool)
            ? pool.Get(position, rotation)
            : null;
    }

    public void Despawn(string poolId, GameObject obj)
    {
        if (_poolRegistry.TryGetValue(poolId, out var pool))
            pool.Return(obj);
    }
}
