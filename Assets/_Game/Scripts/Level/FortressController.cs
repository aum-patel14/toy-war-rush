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
        yield return new WaitForSeconds(0.3f);

        int armyPower = ArmyManager.Instance?.GetTotalArmyPower() ?? 0;
        _currentHP -= armyPower;

        FXManager.Instance?.PlayEffect("FortressHit", transform.position);
        AudioManager.Instance?.PlaySFX("fortress_explode");

        if (_currentHP <= 0 || armyPower >= maxHP)
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
