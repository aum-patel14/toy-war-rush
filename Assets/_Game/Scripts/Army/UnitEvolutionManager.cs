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
        _evolutionCoroutine = StartCoroutine(PlayEvolutionBurst(tier));
    }

    private IEnumerator PlayEvolutionBurst(UnitTier tier)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 pos = player != null ? player.transform.position + Vector3.up : Vector3.zero;

        FloatingTextFx.Instance?.Spawn("EVOLVE!", pos, new Color(1f, 0.9f, 0.35f));
        FXManager.Instance?.PlayEffect("Evolution", pos);
        CameraFollow.Instance?.Shake(0.45f);
        AudioManager.Instance?.PlaySFX("unit_evolve");

        float elapsed = 0f;
        while (elapsed < evolutionAnimDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        _evolutionCoroutine = null;
    }
}
