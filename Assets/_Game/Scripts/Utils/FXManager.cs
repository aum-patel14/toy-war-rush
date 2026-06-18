// TOY WAR RUSH - FXManager.cs
// Plays particle effects by name.

using UnityEngine;
using System.Collections.Generic;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    [System.Serializable]
    public class FXEntry
    {
        public string name;
        public GameObject prefab;
    }

    [SerializeField] private FXEntry[] effects;
    [SerializeField] private int poolSize = 10;

    private readonly Dictionary<string, GenericPool> _pools = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var entry in effects)
        {
            if (entry.prefab != null)
                _pools[entry.name] = new GenericPool(entry.prefab, poolSize, transform);
        }
    }

    public void PlayEffect(string effectName, Vector3 position)
    {
        if (!_pools.TryGetValue(effectName, out var pool)) return;

        var fx = pool.Get(position, Quaternion.identity);
        var particles = fx.GetComponent<ParticleSystem>();
        if (particles != null)
        {
            particles.Play();
            Destroy(fx, particles.main.duration + particles.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(fx, 2f);
        }
    }
}
