// TOY WAR RUSH - ShopManager.cs
// Handles IAP and coin shop transactions.

using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private ShopItemData[] shopItems;

    public event Action<string> OnPurchaseComplete;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool PurchaseWithCoins(ShopItemData item)
    {
        if (item == null || CurrencyManager.Instance == null) return false;
        if (!CurrencyManager.Instance.SpendCoins(item.coinPrice)) return false;

        ApplyReward(item);
        OnPurchaseComplete?.Invoke(item.itemId);
        return true;
    }

    public void PurchaseWithIAP(string productId)
    {
        Debug.Log($"[ShopManager] IAP purchase stub: {productId}");
        AnalyticsManager.Instance?.LogEvent("iap_purchase", new System.Collections.Generic.Dictionary<string, object>
        {
            { "product_id", productId }
        });

        if (productId == "remove_ads")
            AdManager.Instance?.PurchaseRemoveAds();
    }

    public bool PurchaseUpgrade(int index, int baseCost)
    {
        if (SaveManager.Instance == null || CurrencyManager.Instance == null) return false;

        int level = SaveManager.Instance.Data.upgradeLevels[index];
        int cost = Extensions.UpgradeCost(baseCost, level);
        if (!CurrencyManager.Instance.SpendCoins(cost)) return false;

        SaveManager.Instance.Data.upgradeLevels[index]++;
        SaveManager.Instance.Save();
        OnPurchaseComplete?.Invoke($"upgrade_{index}");
        return true;
    }

    private void ApplyReward(ShopItemData item)
    {
        switch (item.rewardType)
        {
            case ShopRewardType.Coins:
                CurrencyManager.Instance?.AddCoins(item.rewardAmount);
                break;
            case ShopRewardType.Gems:
                CurrencyManager.Instance?.AddGems(item.rewardAmount);
                break;
        }
    }
}

[CreateAssetMenu(menuName = "ToyWarRush/ShopItemData", fileName = "ShopItemData_")]
public class ShopItemData : ScriptableObject
{
    public string itemId;
    public string displayName;
    public int coinPrice;
    public string iapProductId;
    public ShopRewardType rewardType;
    public int rewardAmount;
}

public enum ShopRewardType { Coins, Gems, Skin, NoAds }
