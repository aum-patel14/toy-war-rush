// TOY WAR RUSH - FXManager.cs
// Plays particle effects by name.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    private sealed class AutoReturn : MonoBehaviour
    {
        private GenericPool _pool;
        private float _returnAt;

        public void Arm(GenericPool pool, float delay)
        {
            _pool = pool;
            _returnAt = Time.time + Mathf.Max(0.02f, delay);
        }

        private void Update()
        {
            if (_pool == null || Time.time < _returnAt) return;
            _pool.Return(gameObject);
            enabled = false;
        }
    }

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
        var autoReturn = fx.GetComponent<AutoReturn>();
        if (autoReturn == null) autoReturn = fx.AddComponent<AutoReturn>();
        var particles = fx.GetComponent<ParticleSystem>();
        float returnDelay = 0.6f;
        if (particles != null)
        {
            particles.Play();
            returnDelay = particles.main.duration + particles.main.startLifetime.constantMax;
        }
        autoReturn.enabled = true;
        autoReturn.Arm(pool, returnDelay);
    }
}
