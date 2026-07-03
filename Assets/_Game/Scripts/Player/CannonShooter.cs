// TOY WAR RUSH - CannonShooter.cs
// Mob Control style: cannon auto-fires units into the army while playing.

using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] private float fireInterval = 0.18f;
    [SerializeField] private int unitsPerShot = 1;
    [SerializeField] private float holdBoostMultiplier = 0.55f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform formationTarget;

    private float _timer;
    private bool _isHolding;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        _isHolding = Input.GetMouseButton(0) || Input.touchCount > 0;

        float interval = fireInterval * UpgradeRuntime.FireRateMultiplier();
        interval *= _isHolding ? holdBoostMultiplier : 1f;
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        _timer = interval;
        Fire();
    }

    private void Fire()
    {
        if (projectilePrefab != null && muzzle != null)
        {
            var go = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
            var proj = go.GetComponent<CannonProjectile>();
            if (proj != null)
            {
                Vector3 target = formationTarget != null
                    ? formationTarget.position
                    : transform.position + Vector3.forward * 2f;
                target += new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.2f, 0.3f));
                proj.Launch(muzzle.position, target);
            }
        }
        else
        {
            ArmyManager.Instance?.AddUnits(unitsPerShot);
        }

        FXManager.Instance?.PlayEffect("CannonFire", muzzle != null ? muzzle.position : transform.position);
    }
}
