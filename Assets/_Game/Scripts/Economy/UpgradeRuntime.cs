// TOY WAR RUSH - UpgradeRuntime.cs
// Reads saved upgrade levels and exposes gameplay bonuses.

using UnityEngine;

public static class UpgradeRuntime
{
    public const int FireRateIndex = 0;
    public const int StartArmyIndex = 1;
    public const int GateBonusIndex = 2;

    public static int GetLevel(int index)
    {
        var data = SaveManager.Instance?.Data;
        if (data == null || data.upgradeLevels == null || index < 0 || index >= data.upgradeLevels.Length)
            return 0;
        return data.upgradeLevels[index];
    }

    public static float FireRateMultiplier()
    {
        int lvl = GetLevel(FireRateIndex);
        return Mathf.Max(0.45f, 1f - lvl * 0.08f);
    }

    public static int StartArmyBonus() => GetLevel(StartArmyIndex) * 2;

    public static float GateBonusMultiplier()
    {
        int lvl = GetLevel(GateBonusIndex);
        return 1f + lvl * 0.1f;
    }
}
