// TOY WAR RUSH - MergeSystem.cs
// Merges two same-tier units into one next-tier unit.

using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    public static MergeSystem Instance { get; private set; }

    [SerializeField] private float mergeCooldown = 0.2f;

    private float _lastMergeTime;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TryMerge(UnitController a, UnitController b)
    {
        if (a == null || b == null || a == b) return;
        if (Time.time - _lastMergeTime < mergeCooldown) return;
        if (a.CurrentTier != b.CurrentTier) return;

        var config = UnitFactory.Instance?.EvolutionConfig;
        if (config == null) return;

        var data = config.GetDataForTier(a.CurrentTier);
        if (data == null) return;

        UnitTier nextTier = data.evolvesInto;
        if ((int)nextTier <= (int)a.CurrentTier) return;

        Vector3 midpoint = (a.transform.position + b.transform.position) * 0.5f;
        Vector3 localOffset = ArmyManager.Instance.FormationRoot.InverseTransformPoint(midpoint);

        a.Die(silent: true);
        b.Die(silent: true);

        var merged = UnitFactory.Instance.SpawnUnit(nextTier, localOffset);
        if (merged != null)
        {
            FXManager.Instance?.PlayEffect("Merge", midpoint);
            EventBus.Publish(GameEvents.UnitMerged);
            _lastMergeTime = Time.time;
        }
    }
}
