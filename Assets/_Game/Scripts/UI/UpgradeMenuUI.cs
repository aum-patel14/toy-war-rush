// TOY WAR RUSH - UpgradeMenuUI.cs
// Coin-based upgrade tree UI.

using UnityEngine;
using TMPro;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private UpgradeData[] upgrades;
    [SerializeField] private TextMeshProUGUI[] upgradeLevelTexts;
    [SerializeField] private TextMeshProUGUI[] upgradeCostTexts;

    private void OnEnable() => RefreshUI();

    public void RefreshUI()
    {
        if (SaveManager.Instance == null || upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            int level = SaveManager.Instance.Data.upgradeLevels[i];
            int cost = Extensions.UpgradeCost(upgrades[i].baseCost, level);

            if (upgradeLevelTexts != null && i < upgradeLevelTexts.Length && upgradeLevelTexts[i] != null)
                upgradeLevelTexts[i].text = $"Lv.{level}";

            if (upgradeCostTexts != null && i < upgradeCostTexts.Length && upgradeCostTexts[i] != null)
                upgradeCostTexts[i].text = cost.ToString();
        }
    }

    public void PurchaseUpgrade(int index)
    {
        if (upgrades == null || index < 0 || index >= upgrades.Length) return;
        if (SaveManager.Instance == null || CurrencyManager.Instance == null) return;

        int level = SaveManager.Instance.Data.upgradeLevels[index];
        int cost = Extensions.UpgradeCost(upgrades[index].baseCost, level);

        if (!CurrencyManager.Instance.SpendCoins(cost)) return;

        SaveManager.Instance.Data.upgradeLevels[index]++;
        SaveManager.Instance.Save();
        RefreshUI();
    }

    public void OnClose() => gameObject.SetActive(false);
}

[CreateAssetMenu(menuName = "ToyWarRush/UpgradeData", fileName = "UpgradeData_")]
public class UpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    public int baseCost = 100;
    public float valuePerLevel = 1f;
}
