// TOY WAR RUSH - GateController.cs
// Gates modify army size on trigger. Supports +, -, ×, ÷ operations.

using UnityEngine;
using TMPro;
using System.Collections;

public enum GateOperation { Add, Subtract, Multiply, Divide }

public class GateController : MonoBehaviour
{
    [Header("Gate Config")]
    [SerializeField] private GateOperation operation;
    [SerializeField] private float value = 2f;

    [Header("References")]
    [SerializeField] private TextMeshPro valueText;
    [SerializeField] private MeshRenderer gateRenderer;
    [SerializeField] private Material positiveMat;
    [SerializeField] private Material negativeMat;

    private bool _triggered;
    private Color _baseColor = Color.white;

    private void Start()
    {
        if (gateRenderer != null)
            _baseColor = gateRenderer.material.color;
        UpdateVisuals();
    }

    public void Initialize(GateOperation op, float val)
    {
        operation = op;
        value = val;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        bool isPositive = operation == GateOperation.Add || operation == GateOperation.Multiply;

        if (gateRenderer != null && positiveMat != null && negativeMat != null)
            gateRenderer.material = isPositive ? positiveMat : negativeMat;

        if (valueText != null)
        {
            string prefix = operation switch
            {
                GateOperation.Add => "+",
                GateOperation.Subtract => "-",
                GateOperation.Multiply => "×",
                GateOperation.Divide => "÷",
                _ => "+"
            };
            valueText.text = $"{prefix}{value}";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        bool positive = operation == GateOperation.Add || operation == GateOperation.Multiply;
        ApplyEffect();

        string floatLabel = operation switch
        {
            GateOperation.Multiply => $"×{value}",
            GateOperation.Add => $"+{Mathf.RoundToInt(value * UpgradeRuntime.GateBonusMultiplier())}",
            GateOperation.Subtract => $"-{value}",
            GateOperation.Divide => $"÷{value}",
            _ => ""
        };
        FloatingTextFx.Instance?.Spawn(floatLabel, transform.position, positive ? new Color(0.3f, 0.9f, 0.5f) : new Color(1f, 0.35f, 0.35f));
        GameplayJuice.Instance?.OnGatePassed(positive);

        FXManager.Instance?.PlayEffect("GateHit", transform.position);
        AudioManager.Instance?.PlaySFX("gate_pass");
        CameraFollow.Instance?.Shake(0.25f);
        StartCoroutine(GateFlash());
        EventBus.Publish(GameEvents.GatePassed);
        AnalyticsManager.Instance?.LogEvent("gate_hit", new System.Collections.Generic.Dictionary<string, object>
        {
            { "operation", operation.ToString() },
            { "value", value }
        });
    }

    private void ApplyEffect()
    {
        var army = ArmyManager.Instance;
        if (army == null) return;

        switch (operation)
        {
            case GateOperation.Add:
                army.AddUnits(Mathf.RoundToInt(value * UpgradeRuntime.GateBonusMultiplier()));
                break;
            case GateOperation.Subtract:
            {
                int remove = (int)value;
                int current = army.ArmyCount;
                army.RemoveUnits(Mathf.Max(0, Mathf.Min(remove, current - 1)));
                break;
            }
            case GateOperation.Multiply:
                army.MultiplyArmy(value * UpgradeRuntime.GateBonusMultiplier());
                break;
            case GateOperation.Divide:
                army.DivideArmy(value);
                break;
        }
    }

    private IEnumerator GateFlash()
    {
        if (gateRenderer == null) yield break;

        var mat = gateRenderer.material;
        mat.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        mat.color = _baseColor;
    }
}
