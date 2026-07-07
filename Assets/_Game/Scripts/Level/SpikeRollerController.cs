// TOY WAR RUSH - SpikeRollerController.cs
// Mob Control style spike roller — spins, crushes army, rolls forward on hit.

using UnityEngine;
using System.Collections;

public class SpikeRollerController : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 200f;
    [SerializeField] private float crushRollSpeed = 5.5f;
    [SerializeField] private float crushDuration = 0.85f;

    private bool _triggered;
    private Coroutine _crushRoutine;

    private void Update()
    {
        if (_triggered && _crushRoutine != null) return;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        _triggered = true;
        ApplyCrush();
        if (_crushRoutine != null)
            StopCoroutine(_crushRoutine);
        _crushRoutine = StartCoroutine(CrushRoll());
    }

    private void ApplyCrush()
    {
        var army = ArmyManager.Instance;
        if (army == null) return;

        int count = army.ArmyCount;
        int level = LevelManager.Instance?.CurrentLevel ?? 1;
        bool isCave = level >= 11 && level <= 20;
        int loss = Mathf.Max(2, Mathf.RoundToInt(count * (isCave ? 0.42f : 0.28f)));
        army.RemoveUnits(loss);

        if (isCave)
            GameplayJuice.Instance?.TriggerBrutalCrush();
        else
            GameplayJuice.Instance?.TriggerRollerCrush();

        CameraFollow.Instance?.Shake(isCave ? 0.55f : 0.38f);
        FXManager.Instance?.PlayEffect("CrushBurst", transform.position + Vector3.up * 0.6f);
        FXManager.Instance?.PlayEffect("ObstacleHit", transform.position);
        AudioManager.Instance?.PlaySFX("unit_death");
        EventBus.Publish(GameEvents.ObstacleHit);

        Vector3 fxPos = transform.position + Vector3.up * 1.2f;
        FloatingTextFx.Instance?.Spawn($"-{loss}", fxPos, new Color(1f, 0.34f, 0.13f));
        if (isCave)
            FloatingTextFx.Instance?.Spawn("CRUSH!", fxPos + Vector3.up * 0.8f, new Color(1f, 0.92f, 0.23f));
    }

    private IEnumerator CrushRoll()
    {
        float elapsed = 0f;
        float nextFx = 0f;
        while (elapsed < crushDuration)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.back * crushRollSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, spinSpeed * 2.2f * Time.deltaTime, Space.World);
            if (elapsed >= nextFx)
            {
                FXManager.Instance?.PlayEffect("CrushBurst", transform.position + Vector3.up * 0.4f);
                nextFx = elapsed + 0.1f;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
        _crushRoutine = null;
    }
}
