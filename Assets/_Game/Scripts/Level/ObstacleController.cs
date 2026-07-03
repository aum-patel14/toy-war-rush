// TOY WAR RUSH - ObstacleController.cs
// Obstacles damage or destroy units on collision.

using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType;
    [SerializeField] private int unitDamage = 1;
    [SerializeField] private bool destroyOnHit = true;

    public void Initialize(ObstacleType type, int damage)
    {
        obstacleType = type;
        unitDamage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ArmyManager.Instance?.RemoveUnits(unitDamage);
        AudioManager.Instance?.PlaySFX("unit_death");
        CameraFollow.Instance?.Shake(0.4f);
        FXManager.Instance?.PlayEffect("ObstacleHit", transform.position);
        EventBus.Publish(GameEvents.ObstacleHit);

        if (destroyOnHit)
            gameObject.SetActive(false);
    }
}
