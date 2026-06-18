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
        // TODO: Unity IAP integration
        Debug.Log($"[ShopManager] IAP purchase stub: {productId}");
        AnalyticsManager.Instance?.LogEvent("iap_purchase", new System.Collections.Generic.Dictionary<string, object>
        {
            { "product_id", productId }
        });
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
