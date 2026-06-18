// TOY WAR RUSH - UnitEvolutionManager.cs
// Handles bulk evolution animations and particle bursts.

using UnityEngine;
using System.Collections;

public class UnitEvolutionManager : MonoBehaviour
{
    public static UnitEvolutionManager Instance { get; private set; }

    [SerializeField] private float evolutionAnimDuration = 0.4f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);

    private Coroutine _evolutionCoroutine;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        ArmyManager.OnEvolutionTriggered += OnEvolutionTriggered;
    }

    private void OnDisable()
    {
        ArmyManager.OnEvolutionTriggered -= OnEvolutionTriggered;
    }

    private void OnEvolutionTriggered(UnitTier tier)
    {
        if (_evolutionCoroutine != null)
            StopCoroutine(_evolutionCoroutine);
        _evolutionCoroutine = StartCoroutine(PlayEvolutionBurst());
    }

    private IEnumerator PlayEvolutionBurst()
    {
        float elapsed = 0f;
        while (elapsed < evolutionAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / evolutionAnimDuration;
            float scale = scaleCurve.Evaluate(t);
            // Visual pulse handled per-unit in UnitController.Evolve
            yield return null;
        }
        _evolutionCoroutine = null;
    }
}
