// TOY WAR RUSH - CollectibleController.cs
// Lane pickup that adds units to the army.

using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private int armyBonus = 1;
    [SerializeField] private float spinSpeed = 90f;

    private bool _collected;

    public void Initialize(int bonus)
    {
        armyBonus = bonus;
        _collected = false;
    }

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;
        ArmyManager.Instance?.AddUnits(armyBonus);
        FXManager.Instance?.PlayEffect("CollectPickup", transform.position);
        AudioManager.Instance?.PlaySFX("collect");
        CameraFollow.Instance?.Shake(0.15f);
        Destroy(gameObject);
    }
}
