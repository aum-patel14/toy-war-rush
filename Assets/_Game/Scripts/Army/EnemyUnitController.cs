// TOY WAR RUSH - EnemyUnitController.cs
// Red mob that clashes with the player army on contact.

using UnityEngine;

public class EnemyUnitController : MonoBehaviour
{
    [SerializeField] private float marchSpeed = 2.5f;
    [SerializeField] private int clashDamage = 1;

    private bool _dead;

    private void Update()
    {
        if (_dead) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.back * marchSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dead || !other.CompareTag("Player")) return;

        _dead = true;
        ArmyManager.Instance?.RemoveUnits(clashDamage);
        FXManager.Instance?.PlayEffect("ObstacleHit", transform.position);
        Destroy(gameObject);
    }
}
