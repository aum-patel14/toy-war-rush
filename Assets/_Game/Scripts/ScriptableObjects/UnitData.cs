// TOY WAR RUSH - UnitData.cs
// ScriptableObject defining all properties of a unit tier.

using UnityEngine;

public enum UnitTier { ToySoldier, Knight, Robot, Mech, Titan, UltraTitan }

[CreateAssetMenu(menuName = "ToyWarRush/UnitData", fileName = "UnitData_")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public UnitTier tier;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;

    [Header("Stats")]
    public int maxHP = 10;
    public int attackDamage = 5;
    public float attackSpeed = 1f;
    public float attackRange = 2f;
    public float moveRadius = 0.8f;

    [Header("Evolution")]
    public int requiredArmySize = 1;
    public UnitTier evolvesInto;

    [Header("Visual")]
    public float scale = 1f;
    public Color evolutionParticleColor = Color.white;

    [Header("Audio")]
    public AudioClip evolutionSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
}
