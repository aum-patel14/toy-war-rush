// TOY WAR RUSH - GameplayJuice.cs
// Slow-mo, first-gate beat, and other one-shot juice helpers.

using UnityEngine;
using System.Collections;

public class GameplayJuice : MonoBehaviour
{
    public static GameplayJuice Instance { get; private set; }

    private bool _firstGateHitThisLevel;
    private Coroutine _slowMoRoutine;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ResetForLevel()
    {
        _firstGateHitThisLevel = false;
        if (_slowMoRoutine != null)
        {
            StopCoroutine(_slowMoRoutine);
            _slowMoRoutine = null;
        }
        Time.timeScale = 1f;
    }

    public void OnGatePassed(bool positive)
    {
        if (_firstGateHitThisLevel) return;
        _firstGateHitThisLevel = true;
        if (positive)
            _slowMoRoutine = StartCoroutine(FirstGateSlowMo());
    }

    private IEnumerator FirstGateSlowMo()
    {
        Time.timeScale = 0.35f;
        yield return new WaitForSecondsRealtime(0.4f);
        Time.timeScale = 1f;
        _slowMoRoutine = null;
    }

    public void TriggerBrutalCrush()
    {
        if (_slowMoRoutine != null)
            StopCoroutine(_slowMoRoutine);
        _slowMoRoutine = StartCoroutine(BrutalCrushSlowMo());
    }

    public void TriggerRollerCrush()
    {
        if (_slowMoRoutine != null)
            StopCoroutine(_slowMoRoutine);
        _slowMoRoutine = StartCoroutine(RollerCrushSlowMo());
    }

    private IEnumerator BrutalCrushSlowMo()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale = 1f;
        _slowMoRoutine = null;
    }

    private IEnumerator RollerCrushSlowMo()
    {
        Time.timeScale = 0.35f;
        yield return new WaitForSecondsRealtime(0.4f);
        Time.timeScale = 1f;
        _slowMoRoutine = null;
    }
}
