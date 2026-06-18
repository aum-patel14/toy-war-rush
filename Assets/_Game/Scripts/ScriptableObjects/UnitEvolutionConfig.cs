// TOY WAR RUSH - UnitEvolutionConfig.cs
// Maps army count thresholds to unit tiers.

using UnityEngine;

[CreateAssetMenu(menuName = "ToyWarRush/EvolutionConfig", fileName = "UnitEvolutionConfig")]
public class UnitEvolutionConfig : ScriptableObject
{
    public UnitData[] tierData;

    public UnitTier GetTierForCount(int count)
    {
        if (tierData == null || tierData.Length == 0)
            return UnitTier.ToySoldier;

        for (int i = tierData.Length - 1; i >= 0; i--)
        {
            if (tierData[i] != null && count >= tierData[i].requiredArmySize)
                return tierData[i].tier;
        }
        return UnitTier.ToySoldier;
    }

    public UnitData GetDataForTier(UnitTier tier)
    {
        if (tierData == null) return null;
        int index = (int)tier;
        return index >= 0 && index < tierData.Length ? tierData[index] : null;
    }
}
