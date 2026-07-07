// TOY WAR RUSH - EnemyUnitController.cs
// Red mob that clashes with the player army on contact.

using UnityEngine;

public class EnemyUnitController : MonoBehaviour
{
    [SerializeField] private float marchSpeed = 2.5f;
    [SerializeField] private int clashDamage = 2;
    [SerializeField] private float clashCooldown = 0.35f;

    private bool _dead;
    private float _nextClashTime;

    private void Update()
    {
        if (_dead) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.back * marchSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryClash(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryClash(other);
    }

    private void TryClash(Collider other)
    {
        if (_dead || Time.time < _nextClashTime || !other.CompareTag("Player")) return;

        _nextClashTime = Time.time + clashCooldown;
        ArmyManager.Instance?.RemoveUnits(clashDamage);
        FXManager.Instance?.PlayEffect("ObstacleHit", transform.position);
        CameraFollow.Instance?.Shake(0.15f);

        if ((ArmyManager.Instance?.ArmyCount ?? 0) <= 0)
            _dead = true;

        if (Random.value < 0.35f)
        {
            _dead = true;
            Destroy(gameObject);
        }
    }
}
