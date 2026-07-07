// TOY WAR RUSH - GateFramePulse.cs
// Subtle gate glow pulse for Mob Control style math gates.

using UnityEngine;

public class GateFramePulse : MonoBehaviour
{
    [SerializeField] private MeshRenderer panelRenderer;
    [SerializeField] private float pulseSpeed = 2.4f;
    [SerializeField] private float pulseAmount = 0.12f;

    private Material _matInstance;
    private Color _baseColor;
    private float _phase;

    private void Awake()
    {
        if (panelRenderer == null)
            panelRenderer = GetComponentInChildren<MeshRenderer>();
        if (panelRenderer != null)
        {
            _matInstance = panelRenderer.material;
            _baseColor = _matInstance.color;
        }
        _phase = Random.value * Mathf.PI * 2f;
    }

    private void Update()
    {
        if (_matInstance == null) return;
        float t = 1f + Mathf.Sin(Time.time * pulseSpeed + _phase) * pulseAmount;
        _matInstance.color = _baseColor * t;
    }

    private void OnDestroy()
    {
        if (_matInstance != null)
            Destroy(_matInstance);
    }
}
