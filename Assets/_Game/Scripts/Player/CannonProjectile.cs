// TOY WAR RUSH - CannonProjectile.cs
// Visible unit fired from cannon into the army formation.

using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private float arcHeight = 1.4f;

    private Vector3 _start;
    private Vector3 _target;
    private float _t;
    private bool _done;

    public void Launch(Vector3 from, Vector3 to)
    {
        _start = from;
        _target = to;
        _t = 0f;
        _done = false;
        transform.position = from;
    }

    private void Update()
    {
        if (_done) return;

        _t += Time.deltaTime * speed * 0.42f;
        if (_t >= 1f)
        {
            _done = true;
            ArmyManager.Instance?.AddUnits(1);
            FXManager.Instance?.PlayEffect("CannonImpact", _target);
            Destroy(gameObject);
            return;
        }

        var flat = Vector3.Lerp(_start, _target, _t);
        flat.y += Mathf.Sin(_t * Mathf.PI) * arcHeight;
        transform.position = flat;
    }
}
