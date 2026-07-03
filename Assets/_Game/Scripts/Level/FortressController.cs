// TOY WAR RUSH - FortressController.cs
// End-of-level fortress battle — army power vs fortress HP.

using UnityEngine;
using System.Collections;

public class FortressController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float battleDuration = 3f;
    [SerializeField] private Transform battleZone;

    private int _currentHP;
    private bool _battleStarted;

    public void Initialize(int hp)
    {
        maxHP = hp;
        _currentHP = hp;
        _battleStarted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_battleStarted) return;
        if (!other.CompareTag("Player")) return;

        _battleStarted = true;
        StartCoroutine(ResolveBattle());
    }

    private IEnumerator ResolveBattle()
    {
        yield return new WaitForSeconds(0.2f);

        int blue = ArmyManager.Instance?.ArmyCount ?? 0;
        int red = LevelManager.Instance?.CurrentLevelData?.fortressDefenderCount ?? Mathf.Max(10, maxHP / 5);
        if (LevelManager.Instance?.CurrentLevelData?.hasBoss == true)
            red = Mathf.RoundToInt(red * 1.5f);

        float elapsed = 0f;
        while (elapsed < battleDuration && blue > 0 && red > 0)
        {
            int rate = Mathf.Max(1, (blue + red) / 25);
            int blueLoss = Mathf.Min(rate, blue);
            int redLoss = Mathf.Min(rate, red);
            blue -= blueLoss;
            red -= redLoss;
            ArmyManager.Instance?.RemoveUnits(blueLoss);

            FXManager.Instance?.PlayEffect("FortressHit", transform.position + Vector3.up);
            yield return new WaitForSeconds(0.06f);
            elapsed += 0.06f;
        }

        AudioManager.Instance?.PlaySFX("fortress_explode");

        if (blue > red || blue > 0)
        {
            EventBus.Publish(GameEvents.FortressDestroyed);
            Destroy(gameObject, 1f);
        }
        else
        {
            ArmyManager.Instance?.ClearArmy();
            GameManager.Instance?.SetState(GameState.Defeat);
        }
    }
}
