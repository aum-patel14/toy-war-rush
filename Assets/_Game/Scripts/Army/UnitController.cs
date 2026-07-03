// TOY WAR RUSH - UnitController.cs
// Individual unit behavior, evolution visuals, and death.

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitTier currentTier = UnitTier.ToySoldier;

    private UnitData _data;
    private int _currentHP;
    private bool _isDead;

    public UnitTier CurrentTier => currentTier;
    public int CurrentHP => _currentHP;
    public int AttackDamage => _data != null ? _data.attackDamage : 5;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    public void Initialize(UnitData data, Vector3 localOffset)
    {
        _data = data;
        currentTier = data.tier;
        _currentHP = data.maxHP;
        _isDead = false;
        transform.localPosition = localOffset;
        ApplyVisuals();
        ArmyManager.Instance?.RegisterUnit(this);
    }

    public void Evolve(UnitTier newTier)
    {
        if (_isDead) return;

        var config = UnitFactory.Instance?.EvolutionConfig;
        if (config == null) return;

        var newData = config.GetDataForTier(newTier);
        if (newData == null) return;

        currentTier = newTier;
        _data = newData;
        _currentHP = newData.maxHP;
        ApplyVisuals();

        FXManager.Instance?.PlayEffect("Evolution", transform.position);
    }

    private void ApplyVisuals()
    {
        if (_data == null) return;
        transform.localScale = Vector3.one * (_data.scale * 1.18f);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHP -= damage;
        if (_currentHP <= 0)
            Die();
    }

    public void Die(bool silent = false)
    {
        if (_isDead) return;
        _isDead = true;

        if (!silent)
        {
            AudioManager.Instance?.PlaySFX("unit_death");
            FXManager.Instance?.PlayEffect("UnitDeath", transform.position);
        }

        ArmyManager.Instance?.RemoveUnit(this);
        UnitFactory.Instance?.DespawnUnit(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        var otherUnit = other.GetComponent<UnitController>();
        if (otherUnit != null && otherUnit != this && otherUnit.CurrentTier == currentTier)
            MergeSystem.Instance?.TryMerge(this, otherUnit);
    }
}
