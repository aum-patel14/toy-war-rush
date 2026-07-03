// TOY WAR RUSH - MobVisualAnimator.cs
// Gives units a soft blob bounce like Mob Control crowds.

using UnityEngine;

public class MobVisualAnimator : MonoBehaviour
{
    [SerializeField] private float bounceSpeed = 8f;
    [SerializeField] private float bounceAmount = 0.08f;
    [SerializeField] private bool enemyStyle;

    private Vector3 _baseScale;
    private float _phase;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _phase = Random.value * Mathf.PI * 2f;
    }

    private void OnEnable()
    {
        _baseScale = transform.localScale;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _baseScale, Time.deltaTime * 8f);
            return;
        }

        float wave = Mathf.Sin(Time.time * bounceSpeed + _phase);
        float stretch = 1f + wave * bounceAmount;
        float squash = 1f - wave * bounceAmount * 0.6f;
        if (enemyStyle)
            stretch *= 0.97f;

        var target = new Vector3(_baseScale.x * squash, _baseScale.y * stretch, _baseScale.z * squash);
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * 12f);
    }
}
