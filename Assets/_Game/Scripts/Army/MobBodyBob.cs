// TOY WAR RUSH - MobBodyBob.cs
// Soft vertical bob on mob body mesh (Mob Control crowd feel).

using UnityEngine;

public class MobBodyBob : MonoBehaviour
{
    [SerializeField] private float bobSpeed = 9f;
    [SerializeField] private float bobAmount = 0.06f;

    private Vector3 _baseLocalPos;
    private float _phase;

    private void Awake()
    {
        _baseLocalPos = transform.localPosition;
        _phase = Random.value * Mathf.PI * 2f;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        float y = Mathf.Sin(Time.time * bobSpeed + _phase) * bobAmount;
        transform.localPosition = _baseLocalPos + new Vector3(0f, y, 0f);
    }
}
